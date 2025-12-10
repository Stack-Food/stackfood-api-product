using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(c => c.Description)
            .HasColumnName("description");
            
        builder.Property(c => c.DisplayOrder)
            .HasColumnName("display_order")
            .IsRequired();
            
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
            
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(c => c.Name).IsUnique();
    }
}
