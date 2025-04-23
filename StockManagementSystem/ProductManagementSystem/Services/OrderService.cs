using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Enums;
using ProductManagementSystem.Hubs;
using ProductManagementSystem.Models;


namespace ProductManagementSystem.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<StockHub> _hubContext;
        private readonly IProducerService _producer;
        private const int LowStockThreshold = 10;


        public OrderService(ApplicationDbContext context, IHubContext<StockHub> hubContext, IProducerService kafkaProducer)  
        {
            _context = context;
            _hubContext = hubContext;
            _producer = kafkaProducer;
        }

        public async Task<(bool IsSuccess, string Message, int? OrderId, int? RemainingStock)> PlaceOrderAsync(string customerId, int productId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var product = await _context.Products
                    .FromSqlRaw("SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE ProductId = {0}", productId)
                    .FirstOrDefaultAsync();

                if (product == null)
                    return (false, "Product not found.", null, null);

                if (quantity <= 0)
                    return (false, "Invalid quantity requested.", null, product.StockQuantity);

                if (product.StockQuantity < quantity)
                {
                    var failedOrder = new Order
                    {
                        CustomerId = customerId,
                        ProductId = productId,
                        Quantity = quantity,
                        TotalPrice = product.Price * quantity,
                        OrderDate = DateTime.UtcNow,
                        Status = OrderStatus.Failed
                    };

                    _context.Orders.Add(failedOrder);

                    _context.StockEvents.Add(new StockEvent
                    {
                        ProductId = productId,
                        EventType = product.StockQuantity == 0 ? StockEventType.StockInsufficient : StockEventType.OrderFailed,
                        TimeStamp = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return (false, $" Only {product.StockQuantity} left.", null, product.StockQuantity);
                }

                product.StockQuantity -= quantity;

                var order = new Order
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    TotalPrice = product.Price * quantity,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Success
                };

                _context.Orders.Add(order);

                _context.StockEvents.Add(new StockEvent
                {
                    ProductId = productId,
                    EventType = StockEventType.OrderSuccess,
                    TimeStamp = DateTime.UtcNow
                });

                if (product.StockQuantity < LowStockThreshold)
                {
                    var alert = await _context.LowStockAlerts.FirstOrDefaultAsync(a => a.ProductId == product.ProductId);
                    
                    if (alert == null)
                    {
                        _context.LowStockAlerts.Add(new LowStockAlert
                        {
                            ProductId = product.ProductId,
                            Threshold = LowStockThreshold,
                            AlertDate = DateTime.UtcNow
                        });

                        await _producer.PublishLowStockAlertAsync(product.ProductId, product.StockQuantity, product.Name);
                    }
                    else if (alert.Threshold != LowStockThreshold)
                    {
                        alert.Threshold = LowStockThreshold;
                        alert.AlertDate = DateTime.UtcNow;
                        _context.LowStockAlerts.Update(alert);

                        await _producer.PublishLowStockAlertAsync(product.ProductId, product.StockQuantity, product.Name);
                    }

                    _context.StockEvents.Add(new StockEvent
                    {
                        ProductId = productId,
                        EventType = StockEventType.LowStocks,
                        TimeStamp = DateTime.UtcNow
                    });
                }
                else
                {
                    await _producer.PublishStockUpdateAlertAsync(product.ProductId, product.StockQuantity);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Order placed successfully.", order.OrderId, product.StockQuantity);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error: {ex.Message}", null, null);
            }
        }


        public async Task<List<Order>> GetCustomerOrdersAsync(string customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Product)
                .ToListAsync();
        }

    }
}
