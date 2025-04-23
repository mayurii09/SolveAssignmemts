using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using ProductManagementSystem.Data;

namespace ProductManagementSystem.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ApplicationDbContext _context;

        public OrdersController(OrderService orderService, ApplicationDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var result = await _orderService.PlaceOrderAsync(userId, request.ProductId, request.Quantity);

            return Ok(new
            {
                message = result.Message,
                orderId = result.OrderId,
                remainingStock = result.RemainingStock,
                success = result.IsSuccess
            });
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderId,
                    ProductName = o.Product != null ? o.Product.Name : "Product Not Found",
                    o.Quantity,
                    o.TotalPrice,
                    o.OrderDate,
                    Status = o.Status.ToString()
                })
                .ToListAsync();

            Console.WriteLine($"Orders returned for user {userId}: {orders.Count}");
            return Ok(orders);
        }

    }
    public class OrderRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
