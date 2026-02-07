using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Interfaces;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IWebHostEnvironment _environment;

    public BooksController(IBookService bookService, IWebHostEnvironment environment)
    {
        _bookService = bookService;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseModel>>> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponseModel>> GetBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound();
        return Ok(book);
    }

    [HttpGet("author/{author}")]
    public async Task<ActionResult<IEnumerable<BookResponseModel>>> GetBooksByAuthor(string author)
    {
        var books = await _bookService.GetBooksByAuthorAsync(author);
        return Ok(books);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<BookResponseModel>>> GetBooksByCategory(string category)
    {
        var books = await _bookService.GetBooksByCategoryAsync(category);
        return Ok(books);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookResponseModel>> CreateBook([FromBody] BookRequestModel bookDto)
    {
        var createdBook = await _bookService.CreateBookAsync(bookDto);
        return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookResponseModel>> UpdateBook(int id, [FromBody] BookRequestModel bookDto)
    {
        var updatedBook = await _bookService.UpdateBookAsync(id, bookDto);
        if (updatedBook == null)
            return NotFound();
        return Ok(updatedBook);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var result = await _bookService.DeleteBookAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("upload-cover")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> UploadCover(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Only images are allowed.");

        // Validate file size (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size cannot exceed 5MB");

        // Create uploads directory if it doesn't exist
        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "covers");
        Directory.CreateDirectory(uploadsFolder);

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return the relative path
        var relativePath = $"/uploads/covers/{uniqueFileName}";
        return Ok(new { path = relativePath });
    }
}
