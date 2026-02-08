namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown when a book is not found
/// </summary>
public class BookNotFoundException : NotFoundException
{
    public BookNotFoundException(int bookId) 
        : base($"Book with ID {bookId} was not found.")
    {
    }
}
