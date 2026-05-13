using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.ToTable(t => t.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0"));

        builder.Property(p => p.StockQuantity).HasDefaultValue(0);
        builder.ToTable(t => t.HasCheckConstraint("CK_Product_StockQuantity_NonNegative", "[StockQuantity] >= 0"));
        
        builder.Property(p => p.ImageUrl).HasMaxLength(1000);
        builder.Property(p => p.Description).HasColumnType("nvarchar(max)");
        builder.Property(p => p.CreatedDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Product
            {
                Id = 1,
                Name = "iPhone 17 256GB",
                Price = 913.52M,
                StockQuantity = 10,
                CategoryId = 1,
                ImageUrl = "iphone-17-2-400x400.jpg",
                Description = "{\"OS\":\"iOS 26\",\"CPU\":\"A19\",\"GPU\":\"Apple GPU 5 Cores\",\"RAM\":\"8 GB\",\"Memory\":\"256 GB\",\"Cameras\":\"Main: 48.0 MP + 48.0 MP, Front: 18.0 MP\",\"Screen\":\"6.3 inch OLED, 120Hz, Super Retina XDR (1206 x 2622 Pixels), MAX 3000 nits\",\"Battery Duration\":\"30h\",\"SIM Profile\":\"1 eSIM, 1 nano SIM\"}",
                CreatedDate = new DateTime(2026, 5, 6, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2026, 5, 6, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 2,
                Name = "Laptop Lenovo Gaming Legion 5 15IRX10 - 83LY00HYVN (i7 14700HX, 16GB, 1TB, RTX 5050 8GB, WQXGA 165Hz, OfficeH24, Win11)",
                Price = 1636.79M,
                StockQuantity = 5,
                CategoryId = 3,
                ImageUrl = "lenovo-gaming-legion-5-15irx10-i7-14700hx-83ly00hyvn-1-638909844112150811-750x500.jpg",
                Description = "{\"OS\":\"Windows 11\",\"CPU\":\"Intel Core i7 Raptor Lake - 14700HX, 20 Cores 28 Threads, 2.10GHz\",\"GPU\":\"NVIDIA GeForce RTX 5050, 8 GB, 115W\",\"RAM\":\"16 GB DDR5 5600MHz, MAX 32 GB\",\"Hard Drive\":\"1 TB SSD M.2 NVMe PCIe Gen 4 x 4 2242, Support 1 slot for SSD M.2 PCIe (MAX 1TB 2280)\", \"Screen\":\"15.1 inch OLED 165Hz, 100% DCI-P3\"}",
                CreatedDate = new DateTime(2026, 5, 6, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2026, 5, 6, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}