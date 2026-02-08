using System.Diagnostics.CodeAnalysis;

namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown for bad request errors
/// </summary>
[ExcludeFromCodeCoverage]
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
