using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookResponseModel?>
{
    private readonly IBookService _bookService;

    public GetBookByIdQueryHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<BookResponseModel?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        return await _bookService.GetBookByIdAsync(request.Id);
    }
}
