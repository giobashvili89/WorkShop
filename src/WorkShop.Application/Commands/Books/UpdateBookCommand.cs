using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Books;

public record UpdateBookCommand(int Id, BookRequestModel Book) : IRequest<BookResponseModel?>;
