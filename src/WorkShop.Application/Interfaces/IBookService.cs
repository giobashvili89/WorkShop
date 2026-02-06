using WorkShop.Application.DTOs;

namespace WorkShop.Application.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(string author);
    Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category);
    Task<BookDto> CreateBookAsync(BookDto book);
    Task<BookDto?> UpdateBookAsync(int id, BookDto book);
    Task<bool> DeleteBookAsync(int id);
}
