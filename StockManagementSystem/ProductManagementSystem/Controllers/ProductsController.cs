﻿using Microsoft.AspNetCore.Mvc;
using ProductManagementSystem.Services;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(string? search, decimal? maxPrice)
    {
        var products = await _productService.GetFilteredProducts(search, maxPrice);
        return Ok(products);
    }


}
