using ProductManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagementSystem.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetFilteredProducts(string? search, decimal? maxPrice);

    }
}
