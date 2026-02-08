using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Categories;
using WorkShop.Application.Queries.Categories;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseModel>>> GetAllCategories()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseModel>> GetCategory(int id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));
        if (category == null)
            return NotFound();
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponseModel>> CreateCategory([FromBody] CategoryRequestModel categoryDto)
    {
        var createdCategory = await _mediator.Send(new CreateCategoryCommand(categoryDto));
        return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponseModel>> UpdateCategory(int id, [FromBody] CategoryRequestModel categoryDto)
    {
        var updatedCategory = await _mediator.Send(new UpdateCategoryCommand(id, categoryDto));
        if (updatedCategory == null)
            return NotFound();
        return Ok(updatedCategory);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
