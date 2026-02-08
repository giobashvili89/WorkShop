using Microsoft.EntityFrameworkCore;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Enums;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Services;

namespace WorkShop.Infrastructure.Tests;

public class UserServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user1 = new User
        {
            Username = "user1",
            Email = "user1@example.com",
            PasswordHash = "hash",
            Role = UserRole.Customer,
            IsBlocked = false
        };
        var user2 = new User
        {
            Username = "user2",
            Email = "user2@example.com",
            PasswordHash = "hash",
            Role = UserRole.Admin,
            IsBlocked = true
        };
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        var service = new UserService(context);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        var usersList = result.ToList();
        Assert.Equal(2, usersList.Count);
        Assert.Contains(usersList, u => u.Username == "user1");
        Assert.Contains(usersList, u => u.Username == "user2");
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = UserRole.Customer,
            IsBlocked = false
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new UserService(context);

        // Act
        var result = await service.GetUserByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@example.com", result.Email);
        Assert.False(result.IsBlocked);
    }

    [Fact]
    public async Task GetUserByIdAsync_ThrowsUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(
            async () => await service.GetUserByIdAsync(999));
        Assert.Contains("User with ID 999", exception.Message);
    }

    [Fact]
    public async Task BlockUserAsync_BlocksUser_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = UserRole.Customer,
            IsBlocked = false
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new UserService(context);

        // Act
        await service.BlockUserAsync(user.Id);

        // Assert
        var blockedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(blockedUser);
        Assert.True(blockedUser.IsBlocked);
    }

    [Fact]
    public async Task BlockUserAsync_ThrowsUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(
            async () => await service.BlockUserAsync(999));
        Assert.Contains("User with ID 999", exception.Message);
    }

    [Fact]
    public async Task UnblockUserAsync_UnblocksUser_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = UserRole.Customer,
            IsBlocked = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new UserService(context);

        // Act
        await service.UnblockUserAsync(user.Id);

        // Assert
        var unblockedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(unblockedUser);
        Assert.False(unblockedUser.IsBlocked);
    }

    [Fact]
    public async Task UnblockUserAsync_ThrowsUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new UserService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(
            async () => await service.UnblockUserAsync(999));
        Assert.Contains("User with ID 999", exception.Message);
    }
}
