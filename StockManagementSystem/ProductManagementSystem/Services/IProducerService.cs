namespace ProductManagementSystem.Services
{
    public interface IProducerService
    { 
        Task PublishLowStockAlertAsync(int productId, int stockQuantity,string name);

        Task PublishStockUpdateAlertAsync(int productId, int updatedStockQuantity);
    }
}
