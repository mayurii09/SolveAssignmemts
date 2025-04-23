using Confluent.Kafka;
using System.Text.Json;

namespace ProductManagementSystem.Services
{
    public class ProducerService : IProducerService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic; 

        public ProducerService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"]; 
            _topic = configuration["Kafka:Topic"];
        }

        public async Task PublishLowStockAlertAsync(int productId, int stockQuantity, string name)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var message = new
            {
                MessageType = "LowStock", 
                ProductId = productId,
                ProductName = name,
                StockQuantity = stockQuantity,
                AlertMessage = $"Product ID {productId} stock is low: {stockQuantity} left" 
            };

            var serializedMessage = JsonSerializer.Serialize(message);

            await producer.ProduceAsync(_topic, new Message<Null, string> { Value = serializedMessage });

            producer.Flush(TimeSpan.FromSeconds(2));
        }

        public async Task PublishStockUpdateAlertAsync(int productId, int stockQuantity)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var message = new
            {
                MessageType = "StockUpdate",
                ProductId = productId,
                StockQuantity = stockQuantity,
                AlertMessage = $"Stock updated for Product ID {productId}: {stockQuantity} left"
            };

            var serializedMessage = JsonSerializer.Serialize(message);

            await producer.ProduceAsync(_topic, new Message<Null, string> { Value = serializedMessage });

            producer.Flush(TimeSpan.FromSeconds(2));
        }

    }
}
