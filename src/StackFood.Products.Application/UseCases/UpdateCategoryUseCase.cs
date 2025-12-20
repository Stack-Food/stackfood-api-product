using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;

namespace StackFood.Products.Application.UseCases;

public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDTO> ExecuteAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID '{id}' not found.");
        }

        // Verificar se outro categoria já possui o mesmo nome
        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name);
        if (existingCategory != null && existingCategory.Id != id)
        {
            throw new InvalidOperationException($"Another category with name '{request.Name}' already exists.");
        }

        category.Update(request.Name, request.Description, request.DisplayOrder);

        var updatedCategory = await _categoryRepository.UpdateAsync(category);

        return new CategoryDTO
        {
            Id = updatedCategory.Id,
            Name = updatedCategory.Name,
            Description = updatedCategory.Description,
            DisplayOrder = updatedCategory.DisplayOrder
        };
    }
}
