using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using ProductManagementSystem.Hubs;
using ProductManagementSystem.Models;
using System.Text.Json;

public class ConsumerService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IHubContext<StockHub> _hubContext;

    public ConsumerService(IConfiguration config, IHubContext<StockHub> hubContext)
    {
        _config = config;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            GroupId = "product-stock-group",
            BootstrapServers = _config["Kafka:BootstrapServers"],
            AutoOffsetReset = AutoOffsetReset.Earliest,  // .Earliest  .Latest
            EnableAutoCommit = true
        };

        Console.WriteLine("Initializing Kafka consumer...");
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(_config["Kafka:Topic"]);
        Console.WriteLine($"Subscribed to Kafka topic: {_config["Kafka:Topic"]}");

        await Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(1)); 

                    if (result != null && !string.IsNullOrEmpty(result.Message.Value))
                    {
                        Console.WriteLine($"Message received: {result.Message.Value}");

                        var message = JsonSerializer.Deserialize<KafkaMessage>(result.Message.Value);

                        if (message != null)
                        {
                            Console.WriteLine($"Processing message of type: {message.MessageType}");

                            if (message.MessageType == "StockUpdate")
                            {
                                await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", new
                                {
                                    ProductId = message.ProductId,
                                    StockQuantity = message.StockQuantity,
                                    Message = message.AlertMessage
                                });
                                Console.WriteLine("stock update received to consumer");
                            }
                            else if (message.MessageType == "LowStock")
                            {
                                await _hubContext.Clients.All.SendAsync("ReceiveLowStockAlert", new
                                {
                                    ProductId = message.ProductId,
                                    StockQuantity = message.StockQuantity,
                                    ProductName = message.ProductName,
                                    Message = message.AlertMessage
                                });
                                Console.WriteLine("low stock alert generated");
                            }
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Kafka consume error: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }

                await Task.Delay(100, stoppingToken); 
            }

            consumer.Close();
        }, stoppingToken);
    }
}
