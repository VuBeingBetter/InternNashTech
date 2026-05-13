namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}