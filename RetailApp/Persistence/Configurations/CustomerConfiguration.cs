using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;


namespace Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(150);
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(c => c.Email).IsUnique();

        builder.HasData(
            new Customer { 
                Id = 1,
                FirstName = "John",
                LastName = "Retail",
                Email = "johnretail.admin@retail.com",
                PhoneNumber = "0987654321",
                CreatedAt = new DateTime(2026, 5, 6, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}