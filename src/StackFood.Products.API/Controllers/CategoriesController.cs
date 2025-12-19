using Microsoft.AspNetCore.Mvc;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.UseCases;

namespace StackFood.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly GetAllCategoriesUseCase _getAllCategoriesUseCase;

    public CategoriesController(GetAllCategoriesUseCase getAllCategoriesUseCase)
    {
        _getAllCategoriesUseCase = getAllCategoriesUseCase;
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
}
