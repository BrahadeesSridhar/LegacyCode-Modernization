using System;
using System.ComponentModel.DataAnnotations;

namespace LegacyCodeModernization.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid CustomerId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public bool Express { get; set; }
    }
} 