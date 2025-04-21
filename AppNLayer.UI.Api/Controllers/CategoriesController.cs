using AppNLayer.BLL.Services;
using AppNLayer.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AppNLayer.UI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;
    private readonly ILogger<CategoriesController> _logger = logger;


    [HttpGet]
    [ProducesResponseType<IEnumerable<CategoryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id:int}", Name = "GetCategoryById")]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found.", id);
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var createdCategory = await _categoryService.CreateAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category with name {CategoryName}", createDto.Name);
            return BadRequest(new { message = $"Error creating category: {ex.Message}" }); // Avoid exposing too much detail
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateDto, CancellationToken cancellationToken)
    {
        if (id != updateDto.Id)
        {
            return BadRequest("ID mismatch between route parameter and request body.");
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var success = await _categoryService.UpdateAsync(id, updateDto, cancellationToken);
            if (!success)
            {
                _logger.LogWarning("Attempted to update non-existent category with ID {CategoryId}", id);
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
            return BadRequest(new { message = $"Error updating category: {ex.Message}" });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var success = await _categoryService.DeleteAsync(id, cancellationToken);
        if (!success)
        {
            _logger.LogWarning("Delete failed for category ID {CategoryId}. Either not found or failed constraints.", id);
            return NotFound();
        }
        return NoContent();
    }
}
