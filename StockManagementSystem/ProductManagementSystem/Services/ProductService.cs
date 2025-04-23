using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagementSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetFilteredProducts(string? search = null, decimal? maxPrice = null, int? maxStock = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (maxStock.HasValue)
                query = query.Where(p => p.StockQuantity <= maxStock.Value);

            return await query.ToListAsync();
        }
    }
}
