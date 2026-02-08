using Microsoft.EntityFrameworkCore;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Interfaces;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Exceptions;
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
        var books = await _context.Books
            .Include(b => b.Category)
            .ToListAsync();
        return books.Select(b => new BookResponseModel
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            CategoryId = b.CategoryId,
            CategoryName = b.Category?.Name ?? string.Empty,
            Description = b.Description,
            ISBN = b.ISBN,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            SoldCount = b.SoldCount,
            PublishedDate = b.PublishedDate,
            CoverImagePath = b.CoverImagePath
        });
    }

    public async Task<BookResponseModel?> GetBookByIdAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
            throw new BookNotFoundException(id);

        return new BookResponseModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name ?? string.Empty,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            SoldCount = book.SoldCount,
            PublishedDate = book.PublishedDate,
            CoverImagePath = book.CoverImagePath
        };
    }

    public async Task<IEnumerable<BookResponseModel>> GetBooksByAuthorAsync(string author)
    {
        var books = await _context.Books
            .Include(b => b.Category)
            .Where(b => b.Author.ToLower().Contains(author.ToLower()))
            .ToListAsync();

        return books.Select(b => new BookResponseModel
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            CategoryId = b.CategoryId,
            CategoryName = b.Category?.Name ?? string.Empty,
            Description = b.Description,
            ISBN = b.ISBN,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            SoldCount = b.SoldCount,
            PublishedDate = b.PublishedDate,
            CoverImagePath = b.CoverImagePath
        });
    }

    public async Task<IEnumerable<BookResponseModel>> GetBooksByCategoryAsync(string category)
    {
        var books = await _context.Books
            .Include(b => b.Category)
            .Where(b => b.Category != null && b.Category.Name.ToLower().Contains(category.ToLower()))
            .ToListAsync();

        return books.Select(b => new BookResponseModel
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            CategoryId = b.CategoryId,
            CategoryName = b.Category?.Name ?? string.Empty,
            Description = b.Description,
            ISBN = b.ISBN,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            SoldCount = b.SoldCount,
            PublishedDate = b.PublishedDate,
            CoverImagePath = b.CoverImagePath
        });
    }

    public async Task<BookResponseModel> CreateBookAsync(BookRequestModel bookDto)
    {
        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            CategoryId = bookDto.CategoryId,
            Description = bookDto.Description,
            ISBN = bookDto.ISBN,
            Price = bookDto.Price,
            StockQuantity = bookDto.StockQuantity,
            SoldCount = 0,
            PublishedDate = bookDto.PublishedDate,
            CreatedAt = DateTime.UtcNow,
            CoverImagePath = bookDto.CoverImagePath
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Load the category for the response
        await _context.Entry(book).Reference(b => b.Category).LoadAsync();

        return new BookResponseModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name ?? string.Empty,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            SoldCount = book.SoldCount,
            PublishedDate = book.PublishedDate,
            CoverImagePath = book.CoverImagePath
        };
    }

    public async Task<BookResponseModel?> UpdateBookAsync(int id, BookRequestModel bookDto)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
            throw new BookNotFoundException(id);

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.CategoryId = bookDto.CategoryId;
        book.Description = bookDto.Description;
        book.ISBN = bookDto.ISBN;
        book.Price = bookDto.Price;
        book.StockQuantity = bookDto.StockQuantity;
        book.PublishedDate = bookDto.PublishedDate;
        if (bookDto.CoverImagePath != null)
            book.CoverImagePath = bookDto.CoverImagePath;

        await _context.SaveChangesAsync();

        // Reload the category if it was changed
        if (book.Category == null || book.Category.Id != book.CategoryId)
        {
            await _context.Entry(book).Reference(b => b.Category).LoadAsync();
        }

        return new BookResponseModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name ?? string.Empty,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            SoldCount = book.SoldCount,
            PublishedDate = book.PublishedDate,
            CoverImagePath = book.CoverImagePath
        };
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            throw new BookNotFoundException(id);

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
