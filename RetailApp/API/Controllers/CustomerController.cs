using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Persistence.Interfaces;
using Shared.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }
}