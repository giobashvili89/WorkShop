using Microsoft.EntityFrameworkCore;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponseModel>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(u => new UserResponseModel
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role,
            IsBlocked = u.IsBlocked,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task<UserResponseModel> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new UserNotFoundException($"User with ID {id}");

        return new UserResponseModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsBlocked = user.IsBlocked,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task BlockUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new UserNotFoundException($"User with ID {id}");

        user.IsBlocked = true;
        await _context.SaveChangesAsync();
    }

    public async Task UnblockUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new UserNotFoundException($"User with ID {id}");

        user.IsBlocked = false;
        await _context.SaveChangesAsync();
    }
}
