using MeuPetshop.Domain.Dtos.ProductDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuPetShop.Domain.Interfaces.IProducts;
using MeuPetShop.Domain.Shared;

namespace MeuPetshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<ProductDto>>> GetAllProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _productService.GetAllProductsAsync(pageNumber, pageSize);
        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedApiResponse<ProductDto>>> SearchProducts([FromQuery] string term, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return await GetAllProducts(pageNumber, pageSize);
        }
        var response = await _productService.SearchProductsByNameAsync(term, pageNumber, pageSize);
        return Ok(response);
    }
    
    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto productDto)
    {
        try
        {
            var newProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtRoute("GetProductById", new { id = newProduct.Id }, newProduct);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
    {
        var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
        if (updatedProduct == null)
        {
            return NotFound();
        }
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await _productService.DeleteProductAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}