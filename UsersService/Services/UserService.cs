using Microsoft.EntityFrameworkCore;
using UsersService.Data;
using UsersService.Messaging;
using UsersService.Models;

namespace UsersService.Services
{
    public interface IUserService
    {
        Task<User> Create(User user);
        Task<User> GetById(int id);
        Task<List<User>> GetAll();
    }


    public class UserService : IUserService
    {
        private readonly UsersDbContext _context;
        private readonly EventBusPublisher _eventBusPublisher;

        public UserService(UsersDbContext context, EventBusPublisher eventBusPublisher)
        {
            _context = context;
            _eventBusPublisher = eventBusPublisher;
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
    }
}
