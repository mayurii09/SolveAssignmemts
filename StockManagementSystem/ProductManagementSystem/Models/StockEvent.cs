using ProductManagementSystem.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementSystem.Models
{
    public class StockEvent
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        public StockEventType EventType { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        public Product? Product { get; set; }
    }
}
