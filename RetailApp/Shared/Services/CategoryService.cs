using Domain.Entities;
using Persistence.Interfaces;
using Shared.DTOs;
using Shared.Interfaces;

namespace Shared.Services;

public class CategoryService(IRepository<Category> _categoryRepository) : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }


    public async Task CreateAsync(CategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, CategoryDto dto)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null) return;

        existing.Name = dto.Name;
        existing.Description = dto.Description;

        _categoryRepository.Update(existing);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing != null)
        {
            _categoryRepository.Delete(existing);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}