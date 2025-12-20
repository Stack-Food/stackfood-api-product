using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Application.UseCases;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDTO> ExecuteAsync(CreateCategoryRequest request)
    {
        // Verificar se já existe uma categoria com o mesmo nome
        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name);
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"Category with name '{request.Name}' already exists.");
        }

        // Criar a nova categoria
        var category = new Category(
            request.Name,
            request.Description,
            request.DisplayOrder
        );

        var createdCategory = await _categoryRepository.CreateAsync(category);

        return new CategoryDTO
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name,
            Description = createdCategory.Description,
            DisplayOrder = createdCategory.DisplayOrder
        };
    }
}
