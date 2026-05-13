using Microsoft.AspNetCore.Mvc;
using Customer.Services;

namespace Customer.Controllers;

public class ProductController(IProductApiService productApiService) : Controller
{
    private readonly IProductApiService _productApiService = productApiService;

    // Product Catalog
    public async Task<IActionResult> Index()
    {
        var products = await _productApiService.GetProductCatalogAsync();
        return View(products);
    }

    // Product Details
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productApiService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        
        return View(product);
    }
}