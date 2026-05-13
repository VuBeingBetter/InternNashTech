using System.Text.Json;
using Shared.DTOs;
using Shared.ViewModels;

namespace Customer.Services;

public class ProductApiService(HttpClient httpClient) : IProductApiService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<ProductCardViewModel>> GetProductCatalogAsync()
    {
        var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("product");

        // Map sang ViewModel để hiển thị trên Razor
        return products?.Select(p => new ProductCardViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryName = p.CategoryName ?? "Uncategorized"
        }).ToList() ?? [];
    }

    public async Task<ProductDetailsViewModel?> GetProductByIdAsync(int id)
    {
        var product = await _httpClient.GetFromJsonAsync<ProductDto>($"product/{id}");

        if (product == null)
            return null;

        // Parse string to Dict
        var description = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(product.Description))
        {
            try
            {
                description = JsonSerializer.Deserialize<Dictionary<string, string>>(product.Description) 
                        ?? [];
            }
            catch (Exception ex)
            {
                // Log errors
                Console.WriteLine($"Error parsing product description: {ex.Message}");
            }
        }

        return new ProductDetailsViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryName = product.CategoryName ?? "Uncategorized",
            StockQuantity = product.StockQuantity,
            Description = description
        };
    }
}