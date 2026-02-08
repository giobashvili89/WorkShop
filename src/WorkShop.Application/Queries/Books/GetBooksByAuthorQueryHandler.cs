using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public class GetBooksByAuthorQueryHandler : IRequestHandler<GetBooksByAuthorQuery, IEnumerable<BookResponseModel>>
{
    private readonly IBookService _bookService;

    public GetBooksByAuthorQueryHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IEnumerable<BookResponseModel>> Handle(GetBooksByAuthorQuery request, CancellationToken cancellationToken)
    {
        return await _bookService.GetBooksByAuthorAsync(request.Author);
    }
}
