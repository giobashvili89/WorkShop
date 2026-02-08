using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public record GetBooksByAuthorQuery(string Author) : IRequest<IEnumerable<BookResponseModel>>;
