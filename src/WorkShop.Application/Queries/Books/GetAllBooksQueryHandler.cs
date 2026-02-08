using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookResponseModel>>
{
    private readonly IBookService _bookService;

    public GetAllBooksQueryHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IEnumerable<BookResponseModel>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        return await _bookService.GetAllBooksAsync();
    }
}
