using System.Diagnostics.CodeAnalysis;

namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user is not found
/// </summary>
[ExcludeFromCodeCoverage]
public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(int userId) 
        : base($"User with ID {userId} was not found.")
    {
    }

    public UserNotFoundException(string username) 
        : base($"User with username '{username}' was not found.")
    {
    }
}
