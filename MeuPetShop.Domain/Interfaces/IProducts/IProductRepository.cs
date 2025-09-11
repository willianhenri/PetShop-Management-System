using MeuPetShop.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeuPetShop.Domain.Interfaces.IProducts;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task <Product?> GetByIdAsync(int id);
    Task<Product?> GetByNameAsync(string name);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);

    Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<(IEnumerable<Product> Products, int TotalCount)> SearchByNameAsync(string searchTerm, int pageNumber, int pageSize);
}