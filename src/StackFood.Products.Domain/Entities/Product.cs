using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money Price { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property
    public Category Category { get; private set; } = null!;

    // EF Core constructor
    private Product() { }

    public Product(
        Guid categoryId,
        string name,
        string? description,
        Money price,
        string? imageUrl = null,
        bool isAvailable = true)
    {
        Id = Guid.NewGuid();
        CategoryId = categoryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        ImageUrl = imageUrl;
        IsAvailable = isAvailable;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Update(
        Guid categoryId,
        string name,
        string? description,
        Money price,
        string? imageUrl)
    {
        CategoryId = categoryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Product name cannot be empty", nameof(Name));

        if (Name.Length > 200)
            throw new ArgumentException("Product name cannot exceed 200 characters", nameof(Name));

        if (CategoryId == Guid.Empty)
            throw new ArgumentException("Category ID cannot be empty", nameof(CategoryId));

        if (Price == null)
            throw new ArgumentNullException(nameof(Price));
    }
}
