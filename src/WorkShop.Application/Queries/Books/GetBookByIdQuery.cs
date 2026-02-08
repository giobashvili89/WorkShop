using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Books;

public record GetBookByIdQuery(int Id) : IRequest<BookResponseModel?>;
