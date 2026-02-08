using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Books;
using WorkShop.Application.Queries.Books;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _environment;

    public BooksController(IMediator mediator, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseModel>>> GetAllBooks()
    {
        var books = await _mediator.Send(new GetAllBooksQuery());
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponseModel>> GetBook(int id)
    {
        var book = await _mediator.Send(new GetBookByIdQuery(id));
        if (book == null)
            return NotFound();
        return Ok(book);
    }

    [HttpGet("author/{author}")]
    public async Task<ActionResult<IEnumerable<BookResponseModel>>> GetBooksByAuthor(string author)
    {
        var books = await _mediator.Send(new GetBooksByAuthorQuery(author));
        return Ok(books);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<BookResponseModel>>> GetBooksByCategory(string category)
    {
        var books = await _mediator.Send(new GetBooksByCategoryQuery(category));
        return Ok(books);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookResponseModel>> CreateBook([FromBody] BookRequestModel bookDto)
    {
        var createdBook = await _mediator.Send(new CreateBookCommand(bookDto));
        return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookResponseModel>> UpdateBook(int id, [FromBody] BookRequestModel bookDto)
    {
        var updatedBook = await _mediator.Send(new UpdateBookCommand(id, bookDto));
        if (updatedBook == null)
            return NotFound();
        return Ok(updatedBook);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var result = await _mediator.Send(new DeleteBookCommand(id));
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

        // Sanitize filename to prevent path traversal
        var safeFileName = Path.GetFileName(file.FileName);
        
        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(safeFileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Only images are allowed.");

        // Validate file size (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size cannot exceed 5MB");

        // Get web root path
        var webRootPath = _environment.WebRootPath;
        if (string.IsNullOrEmpty(webRootPath))
        {
            webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRootPath);
        }

        // Create uploads directory if it doesn't exist
        var uploadsFolder = Path.Combine(webRootPath, "uploads", "covers");
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
