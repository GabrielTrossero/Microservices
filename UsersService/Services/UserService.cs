using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UsersService.Data;
using UsersService.DTO;
using UsersService.Messaging;
using UsersService.Models;

namespace UsersService.Services
{
    public interface IUserService
    {
        Task<User> Create(User user);
        Task<User> GetById(int id);
        Task<List<User>> GetAll();
        Task<List<UserWithProductsDTO>> GetUsersWithProducts();
    }


    public class UserService : IUserService
    {
        private readonly UsersDbContext _context;
        private readonly EventBusPublisher _eventBusPublisher;
        private readonly HttpClient _http;
        private readonly string _productsBaseUrl;

        public UserService(UsersDbContext context, EventBusPublisher eventBusPublisher, HttpClient http, IConfiguration config)
        {
            _context = context;
            _eventBusPublisher = eventBusPublisher;
            _http = http;
        }

        public async Task<User> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            // Enviar el evento a RabbitMQ
            _eventBusPublisher.PublishUserCreated(user);
            return user;
        }

        public async Task<User> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<List<User>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<List<UserWithProductsDTO>> GetUsersWithProducts()
        {
            var users = await _context.Users.ToListAsync();
            var usersDTO = new List<UserWithProductsDTO>();

            foreach (var user in users)
            {
                var products = await GetProductsByUserId(user.Id);
                usersDTO.Add(new UserWithProductsDTO
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Productos = products
                });
            }
            return usersDTO;
        }

        public async Task<List<ProductDTO>> GetProductsByUserId(int userId)
        {
            var response = await _http.GetAsync($"/api/Products/by-user/{userId}");


            if (!response.IsSuccessStatusCode)
                return new List<ProductDTO>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
