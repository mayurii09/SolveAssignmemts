using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;

namespace ProductManagementSystem.Controllers
{
    [Route("api/alerts")]
    [ApiController]
    public class LowStockAlertController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LowStockAlertController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockAlerts()
        {
            var alerts = await _context.LowStockAlerts
                .Include(a => a.Product)
                .Select(a => new
                {
                    a.AlertId,
                    ProductName = a.Product.Name,
                    a.Threshold,
                    a.AlertDate
                })
                .ToListAsync();

            return Ok(alerts);
        }
    }

}
