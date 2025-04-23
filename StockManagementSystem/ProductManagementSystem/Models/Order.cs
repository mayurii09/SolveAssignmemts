using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProductManagementSystem.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public string? CustomerId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public Product? Product { get; set; }
    }
}
