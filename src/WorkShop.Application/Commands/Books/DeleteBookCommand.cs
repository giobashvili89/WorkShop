using MediatR;

namespace WorkShop.Application.Commands.Books;

public record DeleteBookCommand(int Id) : IRequest<bool>;
