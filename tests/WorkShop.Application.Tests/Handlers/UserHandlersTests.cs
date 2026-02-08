using Moq;
using Xunit;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Queries.Users;
using WorkShop.Application.Commands.Users;
using WorkShop.Domain.Enums;

namespace WorkShop.Application.Tests.Handlers;

public class UserHandlersTests
{
    [Fact]
    public async Task GetAllUsersQueryHandler_ReturnsAllUsers()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        var expectedUsers = new List<UserResponseModel>
        {
            new UserResponseModel { Id = 1, Username = "user1", Email = "user1@example.com", Role = UserRole.Customer, IsBlocked = false },
            new UserResponseModel { Id = 2, Username = "user2", Email = "user2@example.com", Role = UserRole.Admin, IsBlocked = false }
        };
        mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(expectedUsers);
        
        var handler = new GetAllUsersQueryHandler(mockUserService.Object);
        var query = new GetAllUsersQuery();
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockUserService.Verify(s => s.GetAllUsersAsync(), Times.Once);
    }

    [Fact]
    public async Task BlockUserCommandHandler_BlocksUser_Successfully()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(s => s.BlockUserAsync(1)).Returns(Task.CompletedTask);
        
        var handler = new BlockUserCommandHandler(mockUserService.Object);
        var command = new BlockUserCommand(1);
        
        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        mockUserService.Verify(s => s.BlockUserAsync(1), Times.Once);
    }

    [Fact]
    public async Task UnblockUserCommandHandler_UnblocksUser_Successfully()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(s => s.UnblockUserAsync(1)).Returns(Task.CompletedTask);
        
        var handler = new UnblockUserCommandHandler(mockUserService.Object);
        var command = new UnblockUserCommand(1);
        
        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        mockUserService.Verify(s => s.UnblockUserAsync(1), Times.Once);
    }
}
