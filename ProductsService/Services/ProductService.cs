using Microsoft.EntityFrameworkCore;
using ProductsService.Data;
using ProductsService.Models;

namespace ProductsService.Services
{
    public interface IProductService
    {
        Task<Product> Create(Product product);
        Task<Product> GetById(int id);
        Task<List<Product>> GetAll();
        Task AssignDefaultProductToUser(int userId, string userName);
    }


    public class ProductService : IProductService
    {
        private readonly ProductsDbContext _context;

        public ProductService(ProductsDbContext context)
        {
            _context = context;
        }

        public async Task<Product> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product;
        }

        public async Task<List<Product>> GetAll()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task AssignDefaultProductToUser(int userId, string userName)
        {
            var product = new Product
            {
                Nombre = $"Producto para {userName}",
                Precio = 100,
                Id_User = userId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}
