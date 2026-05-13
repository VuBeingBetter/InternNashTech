using Domain.Entities;
using Shared.DTOs;

namespace Shared.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task CreateAsync(CategoryDto dto);
    Task UpdateAsync(int id, CategoryDto dto);
    Task DeleteAsync(int id);
}