using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Categories;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryResponseModel?>;
