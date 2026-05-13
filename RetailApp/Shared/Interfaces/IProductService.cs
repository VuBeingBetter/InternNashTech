using Shared.DTOs;

namespace Shared.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task CreateAsync(ProductDto dto);
    Task UpdateAsync(int id, ProductDto dto);
    Task DeleteAsync(int id);
}