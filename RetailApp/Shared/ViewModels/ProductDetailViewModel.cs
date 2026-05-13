namespace Shared.ViewModels;

public class ProductDetailsViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? CategoryName { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public Dictionary<string, string> Description { get; set; } = new(); // Specification as key-value pairs
}