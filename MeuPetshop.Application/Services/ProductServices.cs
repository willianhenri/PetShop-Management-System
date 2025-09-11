using MeuPetshop.Domain.Dtos;
using MeuPetshop.Domain.Dtos.ProductDtos;
using MeuPetShop.Domain.Entities;
using MeuPetShop.Domain.Interfaces;
using MeuPetShop.Domain.Interfaces.IProducts;
using MeuPetShop.Domain.Shared;

namespace MeuPetshop.Application.Services;

public class ProductServices : IProductService
{
     private readonly IProductRepository _productRepository;

    public ProductServices(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        var existingProduct = await _productRepository.GetByNameAsync(productDto.Name);
        if (existingProduct != null)
        {
            throw new InvalidOperationException("Já existe um produto com este nome.");
        }

        var newProduct = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            StockQuantity = productDto.StockQuantity,
            DateAdded = DateTime.UtcNow
        };

        await _productRepository.AddAsync(newProduct);
        return MapProductToDto(newProduct);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapProductToDto(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        var productToUpdate = await _productRepository.GetByIdAsync(id);
        if (productToUpdate == null) return null;

        productToUpdate.Name = productDto.Name;
        productToUpdate.Description = productDto.Description;
        productToUpdate.Price = productDto.Price;
        productToUpdate.StockQuantity = productDto.StockQuantity;

        await _productRepository.UpdateAsync(productToUpdate);
        return MapProductToDto(productToUpdate);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var productToDelete = await _productRepository.GetByIdAsync(id);
        if (productToDelete == null) return false;

        await _productRepository.DeleteAsync(productToDelete);
        return true;
    }

    public async Task<PagedApiResponse<ProductDto>> GetAllProductsAsync(int pageNumber, int pageSize)
    {
        var (products, totalCount) = await _productRepository.GetAllAsync(pageNumber, pageSize);
        var productDtos = products.Select(MapProductToDto);

        return new PagedApiResponse<ProductDto>
        {
            Data = productDtos,
            Pagination = new PaginationData
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }

    public async Task<PagedApiResponse<ProductDto>> SearchProductsByNameAsync(string searchTerm, int pageNumber, int pageSize)
    {
        var (products, totalCount) = await _productRepository.SearchByNameAsync(searchTerm, pageNumber, pageSize);
        var productDtos = products.Select(MapProductToDto);

        return new PagedApiResponse<ProductDto>
        {
            Data = productDtos,
            Pagination = new PaginationData
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }

    private ProductDto MapProductToDto(Product product)
    {
        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.DateAdded);
    }
    
}