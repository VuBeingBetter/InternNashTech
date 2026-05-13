using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Domain.Entities;
using Persistence.Interfaces;

namespace Persistence.Repository;

public class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
        => await _dbSet.Where(p => p.CategoryId == categoryId)
                       .Include(p => p.Category)
                       .ToListAsync();
    
    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet.Include(p => p.Category).ToListAsync();
    }
}
