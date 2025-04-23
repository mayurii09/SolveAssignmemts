using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementSystem.Models
{
    public class LowStockAlert
    {
        [Key]
        public int AlertId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Threshold must be greater than 0")]
        public int Threshold { get; set; }

        [Required]
        public DateTime AlertDate { get; set; }

        public Product? Product { get; set; }
    }
}


