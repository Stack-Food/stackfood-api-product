using Microsoft.EntityFrameworkCore;
using StackFood.Products.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace StackFood.Products.Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
