namespace ProductManagementSystem.Models
{
    public class KafkaMessage
    {
        public string? MessageType { get; set; } 
        public int ProductId { get; set; }
        public int StockQuantity { get; set; }
        public string? AlertMessage { get; set; } 
        public string? ProductName { get; set; }
    }

}
