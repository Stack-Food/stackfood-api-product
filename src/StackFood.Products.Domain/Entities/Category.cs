namespace StackFood.Products.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    // EF Core constructor
    private Category() { }

    public Category(string name, string? description, int displayOrder)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        DisplayOrder = displayOrder;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Update(string name, string? description, int displayOrder)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Category name cannot be empty", nameof(Name));

        if (Name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(Name));

        if (DisplayOrder < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(DisplayOrder));
    }
}
