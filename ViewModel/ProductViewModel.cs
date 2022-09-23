

using System;
using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.ViewModel
{
    public class ProductViewModel
    {        
        public int? ProductCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public int UnitPrice { get; set; }
        public string ManufactureDate { get; set; }
        
        public bool IsActive { get; set; }
        
        public  string AddedDate { get; set; }

        public string LastEditedDate { get; set; }
    }

    public class ProductUpdateModel : ProductCreateModel
    {
        [Required]
        public new string ManufactureDate { get; set; }

        public new string AddedDate { get; set; }

    }

    public class ProductCreateModel : ProductViewModel 
    {

    }
    public class ProductDeleteModel : ProductViewModel
    {

    }



}
