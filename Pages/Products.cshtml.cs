
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PracticeWeb.ViewModel;
using System.Collections.Generic;

namespace PracticeWeb.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ILogger<ProductsModel> _logger;

        public IEnumerable<ProductViewModel> Product { get; set; }

        public ProductsModel(ILogger<ProductsModel> logger)
        {
            _logger = logger;
        }

        //public void OnGet()
        //{
        //    return Page();
        //}

    }
}
