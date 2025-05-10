using Microsoft.EntityFrameworkCore;
using ProductsService.Models;
using System.Collections.Generic;

namespace ProductsService.Data
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
    }
}

