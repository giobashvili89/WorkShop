using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Categories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryResponseModel>>;
