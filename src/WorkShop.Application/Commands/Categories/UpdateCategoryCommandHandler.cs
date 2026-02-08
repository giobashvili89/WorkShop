using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Categories;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponseModel?>
{
    private readonly ICategoryService _categoryService;

    public UpdateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryResponseModel?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryService.UpdateCategoryAsync(request.Id, request.Category);
    }
}
