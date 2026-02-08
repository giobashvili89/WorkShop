using System.Diagnostics.CodeAnalysis;

namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown for unauthorized access attempts
/// </summary>
[ExcludeFromCodeCoverage]
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access.") : base(message)
    {
    }
}
