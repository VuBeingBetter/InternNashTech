using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);

        builder.HasMany(c => c.Products)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId);


        builder.HasData(
            new Category
            {
                Id = 1,
                Name = "Smartphone",
                Description = "Smartphones"
            },
            new Category
            {
                Id = 2,
                Name = "Tablet",
                Description = "Tablets"
            },
            new Category
            {
                Id = 3,
                Name = "Laptop",
                Description = "Laptops"
            }
        );
    }
}