using System;
using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Models
{
    public class Product
    {
        [Key]
        public int ProductCode { get; set; }
        [Required]
        public string Name { get; set; }
        public int UnitPrice { get; set; }
        
        public DateTime ManufactureDate { get; set; }
        
        [Required]
        public bool IsActive { get; set; }
        public DateTime LastEditedDate { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }

        public int CategoryId { get; set; }

        public ProductCategory Category { get; set; }
    }
}
