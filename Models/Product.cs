using System;
using System.ComponentModel.DataAnnotations;

namespace LegacyCodeModernization.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
} 