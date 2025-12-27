using StackFood.Products.Application.Interfaces;

namespace StackFood.Products.Application.UseCases;

public class DeleteCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        var exists = await _categoryRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Category with ID '{id}' not found.");
        }

        await _categoryRepository.DeleteAsync(id);
    }
}
