using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Books;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookResponseModel?>
{
    private readonly IBookService _bookService;

    public UpdateBookCommandHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<BookResponseModel?> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        return await _bookService.UpdateBookAsync(request.Id, request.Book);
    }
}
