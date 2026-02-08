using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public record GetAllBooksQuery : IRequest<IEnumerable<BookResponseModel>>;
