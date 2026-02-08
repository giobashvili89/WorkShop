namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown for bad request errors
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
