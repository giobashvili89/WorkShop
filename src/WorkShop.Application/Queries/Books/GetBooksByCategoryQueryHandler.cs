using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public class GetBooksByCategoryQueryHandler : IRequestHandler<GetBooksByCategoryQuery, IEnumerable<BookResponseModel>>
{
    private readonly IBookService _bookService;

    public GetBooksByCategoryQueryHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IEnumerable<BookResponseModel>> Handle(GetBooksByCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _bookService.GetBooksByCategoryAsync(request.Category);
    }
}
