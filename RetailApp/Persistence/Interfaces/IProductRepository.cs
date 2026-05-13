using Domain.Entities;

namespace Persistence.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
}