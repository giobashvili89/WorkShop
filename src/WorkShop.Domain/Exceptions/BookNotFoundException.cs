using System.Diagnostics.CodeAnalysis;

namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown when a book is not found
/// </summary>
[ExcludeFromCodeCoverage]
public class BookNotFoundException : NotFoundException
{
    public BookNotFoundException(int bookId) 
        : base($"Book with ID {bookId} was not found.")
    {
    }
}
