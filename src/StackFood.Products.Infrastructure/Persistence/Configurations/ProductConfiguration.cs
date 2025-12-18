using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackFood.Products.Domain.Entities;
using StackFood.Products.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace StackFood.Products.Infrastructure.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();
            
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(p => p.Description)
            .HasColumnName("description");
            
        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("price")
                .HasPrecision(10, 2)
                .IsRequired();
        });
            
        builder.Property(p => p.ImageUrl)
            .HasColumnName("image_url");
            
        builder.Property(p => p.IsAvailable)
            .HasColumnName("is_available")
            .IsRequired();
            
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
            
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.IsAvailable);
    }
}
