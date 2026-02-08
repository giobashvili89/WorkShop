using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Books;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookResponseModel>
{
    private readonly IBookService _bookService;

    public CreateBookCommandHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<BookResponseModel> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        return await _bookService.CreateBookAsync(request.Book);
    }
}
