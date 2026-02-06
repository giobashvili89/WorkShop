using Microsoft.EntityFrameworkCore;
using WorkShop.Application.DTOs;
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

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _context.Books.ToListAsync();
        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category,
            Description = b.Description,
            PublishedDate = b.PublishedDate
        });
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return null;

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            Description = book.Description,
            PublishedDate = book.PublishedDate
        };
    }

    public async Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(string author)
    {
        var books = await _context.Books
            .Where(b => b.Author.ToLower().Contains(author.ToLower()))
            .ToListAsync();

        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category,
            Description = b.Description,
            PublishedDate = b.PublishedDate
        });
    }

    public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category)
    {
        var books = await _context.Books
            .Where(b => b.Category.ToLower().Contains(category.ToLower()))
            .ToListAsync();

        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category,
            Description = b.Description,
            PublishedDate = b.PublishedDate
        });
    }

    public async Task<BookDto> CreateBookAsync(BookDto bookDto)
    {
        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Category = bookDto.Category,
            Description = bookDto.Description,
            PublishedDate = bookDto.PublishedDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        bookDto.Id = book.Id;
        return bookDto;
    }

    public async Task<BookDto?> UpdateBookAsync(int id, BookDto bookDto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return null;

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Category = bookDto.Category;
        book.Description = bookDto.Description;
        book.PublishedDate = bookDto.PublishedDate;

        await _context.SaveChangesAsync();

        bookDto.Id = id;
        return bookDto;
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
