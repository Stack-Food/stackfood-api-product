using StackFood.Products.Application.Interfaces;

namespace StackFood.Products.Application.UseCases;

public class DeleteProductUseCase
{
    private readonly IProductRepository _productRepository;

    public DeleteProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        var exists = await _productRepository.ExistsAsync(id);
        if (!exists)
            throw new ArgumentException($"Product with ID {id} not found");

        await _productRepository.DeleteAsync(id);
    }
}
