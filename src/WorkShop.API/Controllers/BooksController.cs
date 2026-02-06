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

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
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
}
