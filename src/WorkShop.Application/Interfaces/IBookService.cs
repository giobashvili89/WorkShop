using WorkShop.Application.Models;

namespace WorkShop.Application.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookResponseModel>> GetAllBooksAsync();
    Task<BookResponseModel?> GetBookByIdAsync(int id);
    Task<IEnumerable<BookResponseModel>> GetBooksByAuthorAsync(string author);
    Task<IEnumerable<BookResponseModel>> GetBooksByCategoryAsync(string category);
    Task<BookResponseModel> CreateBookAsync(BookRequestModel book);
    Task<BookResponseModel?> UpdateBookAsync(int id, BookRequestModel book);
    Task<bool> DeleteBookAsync(int id);
}
