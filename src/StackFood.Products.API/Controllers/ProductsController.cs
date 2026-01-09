using Microsoft.AspNetCore.Mvc;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.UseCases;

namespace StackFood.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly GetAllProductsUseCase _getAllProductsUseCase;
    private readonly GetProductByIdUseCase _getProductByIdUseCase;
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly UpdateProductUseCase _updateProductUseCase;
    private readonly DeleteProductUseCase _deleteProductUseCase;

    public ProductsController(
        GetAllProductsUseCase getAllProductsUseCase,
        GetProductByIdUseCase getProductByIdUseCase,
        CreateProductUseCase createProductUseCase,
        UpdateProductUseCase updateProductUseCase,
        DeleteProductUseCase deleteProductUseCase)
    {
        _getAllProductsUseCase = getAllProductsUseCase;
        _getProductByIdUseCase = getProductByIdUseCase;
        _createProductUseCase = createProductUseCase;
        _updateProductUseCase = updateProductUseCase;
        _deleteProductUseCase = deleteProductUseCase;
    }

    /// <summary>
    /// List all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll(
        [FromQuery] string? category = null,
        [FromQuery] bool? available = null)
    {
        try
        {
            var products = await _getAllProductsUseCase.ExecuteAsync(category, available);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving products", error = ex.Message });
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetById(Guid id)
    {
        try
        {
            var product = await _getProductByIdUseCase.ExecuteAsync(id);

            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving product", error = ex.Message });
        }
    }

    /// <summary>
    /// Get products by category name
    /// </summary>
    [HttpGet("category/{categoryName}")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetByCategory(string categoryName)
    {
        try
        {
            var products = await _getAllProductsUseCase.ExecuteAsync(categoryName);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving products", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDTO>> Create([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _createProductUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating product", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDTO>> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _updateProductUseCase.ExecuteAsync(id, request);
            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating product", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _deleteProductUseCase.ExecuteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting product", error = ex.Message });
        }
    }
}
