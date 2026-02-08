using MediatR;

namespace WorkShop.Application.Commands.Categories;

public record DeleteCategoryCommand(int Id) : IRequest<bool>;
