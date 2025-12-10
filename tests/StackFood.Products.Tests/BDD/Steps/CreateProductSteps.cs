using FluentAssertions;
using Moq;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;
using TechTalk.SpecFlow;

namespace StackFood.Products.Tests.BDD.Steps;

[Binding]
public class CreateProductSteps
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateProductUseCase _useCase;
    private Guid _categoryId;
    private ProductDTO? _result;
    private Exception? _exception;

    public CreateProductSteps()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _useCase = new CreateProductUseCase(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Given(@"I have a valid category ""(.*)""")]
    public void GivenIHaveAValidCategory(string categoryName)
    {
        _categoryId = Guid.NewGuid();
        var category = new Category(categoryName, "Description", 1);

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(_categoryId))
            .ReturnsAsync(true);
    }

    [When(@"I create a product with name ""(.*)"" and price (.*)")]
    public async Task WhenICreateAProductWithNameAndPrice(string name, decimal price)
    {
        var request = new CreateProductRequest
        {
            CategoryId = _categoryId,
            Name = name,
            Description = "Test product",
            Price = price,
            IsAvailable = true
        };

        _productRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        try
        {
            _result = await _useCase.ExecuteAsync(request);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the product should be created successfully")]
    public void ThenTheProductShouldBeCreatedSuccessfully()
    {
        _exception.Should().BeNull();
        _result.Should().NotBeNull();
    }

    [Then(@"the product should have name ""(.*)""")]
    public void ThenTheProductShouldHaveName(string expectedName)
    {
        _result!.Name.Should().Be(expectedName);
    }

    [Then(@"the product should have price (.*)")]
    public void ThenTheProductShouldHavePrice(decimal expectedPrice)
    {
        _result!.Price.Should().Be(expectedPrice);
    }
}
