using GrpcProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcProductService.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
