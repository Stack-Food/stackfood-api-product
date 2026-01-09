using Microsoft.AspNetCore.Mvc;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.UseCases;
using System.Diagnostics.CodeAnalysis;

namespace StackFood.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ExcludeFromCodeCoverage]
public class CategoriesController : ControllerBase
{
    private readonly GetAllCategoriesUseCase _getAllCategoriesUseCase;
    private readonly GetCategoryByIdUseCase _getCategoryByIdUseCase;
    private readonly CreateCategoryUseCase _createCategoryUseCase;
    private readonly UpdateCategoryUseCase _updateCategoryUseCase;
    private readonly DeleteCategoryUseCase _deleteCategoryUseCase;

    public CategoriesController(
        GetAllCategoriesUseCase getAllCategoriesUseCase,
        GetCategoryByIdUseCase getCategoryByIdUseCase,
        CreateCategoryUseCase createCategoryUseCase,
        UpdateCategoryUseCase updateCategoryUseCase,
        DeleteCategoryUseCase deleteCategoryUseCase)
    {
        _getAllCategoriesUseCase = getAllCategoriesUseCase;
        _getCategoryByIdUseCase = getCategoryByIdUseCase;
        _createCategoryUseCase = createCategoryUseCase;
        _updateCategoryUseCase = updateCategoryUseCase;
        _deleteCategoryUseCase = deleteCategoryUseCase;
    }

    /// <summary>
    /// List all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
    {
        try
        {
            var categories = await _getAllCategoriesUseCase.ExecuteAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving categories", error = ex.Message });
        }
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDTO>> GetById(Guid id)
    {
        try
        {
            var category = await _getCategoryByIdUseCase.ExecuteAsync(id);

            if (category == null)
                return NotFound(new { message = $"Category with ID '{id}' not found." });

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving category", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var category = await _createCategoryUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating category", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDTO>> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _updateCategoryUseCase.ExecuteAsync(id, request);
            return Ok(category);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating category", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _deleteCategoryUseCase.ExecuteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting category", error = ex.Message });
        }
    }
}
