using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Categories;

public record UpdateCategoryCommand(int Id, CategoryRequestModel Category) : IRequest<CategoryResponseModel?>;
