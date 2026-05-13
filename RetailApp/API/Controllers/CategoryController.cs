using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() {
        var categories = await categoryService.GetAllAsync();
        if (categories == null) return NotFound(new { message = "No categories found" });
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category == null) return NotFound(new { message = "Category not found" });
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        await categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoryDto dto)
    {
        if (id != dto.Id) return BadRequest("Id mismatch");
        await categoryService.UpdateAsync(id, dto);
        return Ok(new { message = "Updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await categoryService.DeleteAsync(id);
        return NoContent();
    }

}