using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;

namespace StackFood.Products.Application.UseCases;

public class GetCategoryByIdUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDTO?> ExecuteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return null;

        return new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder
        };
    }
}
