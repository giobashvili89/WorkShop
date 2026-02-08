namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Base exception for resource not found errors
/// </summary>
public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message) : base(message)
    {
    }
}
