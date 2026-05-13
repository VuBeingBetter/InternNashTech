using Persistence.Interfaces;
using Domain.Entities;
using Shared.DTOs;
using Shared.Interfaces;

namespace Shared.Services;

public class ProductService(IProductRepository productRepository, IFileService fileService) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileService _fileService = fileService;


    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            CategoryId = p.Category.Id,
            CategoryName = p.Category?.Name ?? "Uncategorized",
            CreatedDate = p.CreatedDate,
            UpdatedDate = p.UpdatedDate 
        });
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            CategoryId = product.Category.Id,
            CategoryName = product.Category?.Name ?? "Uncategorized",
            CreatedDate = product.CreatedDate,
            UpdatedDate = product.UpdatedDate 
        };
    }

    public async Task CreateAsync(ProductDto dto)
    {
        string fileName = "";
        if (dto.ImageFile != null)
        {
            fileName = await _fileService.SaveFileAsync(dto.ImageFile, "images/products");
            Console.WriteLine($"File saved with name: {fileName}");
        }
        else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            fileName = dto.ImageUrl;
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Description = dto.Description,
            ImageUrl = fileName,
            CategoryId = dto.CategoryId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);

        await _productRepository.SaveChangesAsync();
        
    }

    public async Task UpdateAsync(int id, ProductDto dto)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null) return;

        string fileName = existing.ImageUrl ?? "";
        if (dto.ImageFile != null)
        {
            if (!string.IsNullOrWhiteSpace(existing.ImageUrl))
            {
                _fileService.DeleteFile(existing.ImageUrl, "images/products");
            }
            fileName = await _fileService.SaveFileAsync(dto.ImageFile, "images/products");
        }
        else if (!string.IsNullOrWhiteSpace(dto.ImageUrl) && dto.ImageUrl != existing.ImageUrl)
        {
            if (!string.IsNullOrWhiteSpace(existing.ImageUrl))
            {
                _fileService.DeleteFile(existing.ImageUrl, "images/products");
            }
            fileName = dto.ImageUrl;
        }

        existing.Name = dto.Name;
        existing.Price = dto.Price;
        existing.StockQuantity = dto.StockQuantity;
        existing.Description = dto.Description;
        existing.ImageUrl = fileName;
        existing.CategoryId = dto.CategoryId;
        existing.UpdatedDate = DateTime.UtcNow;

        _productRepository.Update(existing);

        await _productRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing != null)
        {
            _productRepository.Delete(existing);
            await _productRepository.SaveChangesAsync();
        }
    }
}