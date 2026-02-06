using Microsoft.EntityFrameworkCore;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Interfaces;
using WorkShop.Domain.Entities;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookResponseModel>> GetAllBooksAsync()
    {
        var books = await _context.Books.ToListAsync();
        return books.Select(b => new BookResponseModel
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category,
            Description = b.Description,
            ISBN = b.ISBN,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            SoldCount = b.SoldCount,
            PublishedDate = b.PublishedDate
        });
    }

    public async Task<BookResponseModel?> GetBookByIdAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return null;

        return new BookResponseModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            SoldCount = book.SoldCount,
            PublishedDate = book.PublishedDate
        };
    }

    public async Task<IEnumerable<BookResponseModel>> GetBooksByAuthorAsync(string author)
    {
        var books = await _context.Books
            .Where(b => b.Author.ToLower().Contains(author.ToLower()))
            .ToListAsync();

        return books.Select(b => new BookResponseModel
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category,
            Description = b.Description,
            ISBN = b.ISBN,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            SoldCount = b.SoldCount,
            PublishedDate = b.PublishedDate
        });
    }

    public async Task<IEnumerable<BookResponseModel>> GetBooksByCategoryAsync(string category)
    {
        var books = await _context.Books
            .Where(b => b.Category.ToLower().Contains(category.ToLower()))
            .ToListAsync();

        return books.Select(b => new BookResponseModel
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category,
            Description = b.Description,
            ISBN = b.ISBN,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            SoldCount = b.SoldCount,
            PublishedDate = b.PublishedDate
        });
    }

    public async Task<BookResponseModel> CreateBookAsync(BookRequestModel bookDto)
    {
        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Category = bookDto.Category,
            Description = bookDto.Description,
            ISBN = bookDto.ISBN,
            Price = bookDto.Price,
            StockQuantity = bookDto.StockQuantity,
            SoldCount = 0,
            PublishedDate = bookDto.PublishedDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return new BookResponseModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            SoldCount = book.SoldCount,
            PublishedDate = book.PublishedDate
        };
    }

    public async Task<BookResponseModel?> UpdateBookAsync(int id, BookRequestModel bookDto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return null;

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Category = bookDto.Category;
        book.Description = bookDto.Description;
        book.ISBN = bookDto.ISBN;
        book.Price = bookDto.Price;
        book.StockQuantity = bookDto.StockQuantity;
        book.PublishedDate = bookDto.PublishedDate;

        await _context.SaveChangesAsync();

        return new BookResponseModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            SoldCount = book.SoldCount,
            PublishedDate = book.PublishedDate
        };
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
