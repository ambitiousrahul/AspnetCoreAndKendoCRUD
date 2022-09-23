
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Models;


namespace PracticeWeb.Data
{
    public class PracticeWebDBContext : DbContext
    {
        public PracticeWebDBContext (DbContextOptions<PracticeWebDBContext> options)
            : base(options)
        {
        }

        public DbSet<PracticeWeb.Models.Product> Product { get; set; }

        public DbSet<PracticeWeb.Models.ProductCategory> ProductCategory { get; set; }
    }
}
