
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Models
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime LastEditedDate { get; set; }

        public virtual IReadOnlyCollection<Product> Products { get; set; }
    }
}
