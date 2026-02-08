using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Interfaces;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Enums;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseModel?> RegisterAsync(RegisterRequestModel registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            throw new BadRequestException("User with this username or email already exists.");

        var passwordHash = PasswordHasher.HashPassword(registerDto.Password);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Customer, 
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseModel
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            UserId = user.Id
        };
    }

    public async Task<AuthResponseModel?> LoginAsync(LoginRequestModel loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user == null)
            throw new UserNotFoundException(loginDto.Username);

        if (!PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        var token = GenerateJwtToken(user);

        return new AuthResponseModel
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            UserId = user.Id
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequestModel request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            throw new UserNotFoundException($"User with email '{request.Email}' was not found.");

        // Generate a secure reset token
        var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

        await _context.SaveChangesAsync();

        // In a real application, you would send an email with the reset link here
        // For now, we'll return the token (in production, this should be sent via email)
        return resetToken;
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestModel request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
        if (user == null)
            throw new BadRequestException("Invalid reset token.");

        if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Reset token has expired.");

        // Update password
        user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await _context.SaveChangesAsync();
    }
}
