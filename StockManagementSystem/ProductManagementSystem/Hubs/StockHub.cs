using Microsoft.AspNetCore.SignalR;

namespace ProductManagementSystem.Hubs
{
    public class StockHub : Hub
    {
        public async Task SendStockUpdate(int productId, int stockQuantity)
        {
            await Clients.All.SendAsync("ReceiveStockUpdateAlert", productId, stockQuantity);
        }

        public async Task SendLowStockAlert(int productId, int stockQuantity, string message)
        {
            await Clients.All.SendAsync("ReceiveLowStockAlert", new
            {
                ProductId = productId,
                StockQuantity = stockQuantity,
                Message = message
            });
        }
    }
}
