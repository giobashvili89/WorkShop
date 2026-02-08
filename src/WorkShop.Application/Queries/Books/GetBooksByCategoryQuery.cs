using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public record GetBooksByCategoryQuery(string Category) : IRequest<IEnumerable<BookResponseModel>>;
