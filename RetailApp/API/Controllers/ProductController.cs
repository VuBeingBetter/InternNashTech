using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() {
        var products = await productService.GetAllAsync();
        if (products == null) return NotFound(new { message = "No products found" });
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductDto dto)
    {
        await productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductDto dto)
    {
        if (id != dto.Id) return BadRequest("Id mismatch");
        await productService.UpdateAsync(id, dto);
        return Ok(new { message = "Updated successfully"});
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await productService.DeleteAsync(id);
        return NoContent();
    }
}