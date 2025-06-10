using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsersService.Data;
using UsersService.Messaging;
using UsersService.Models;

namespace UsersService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersDbContext _context;
        private readonly EventBusPublisher _eventBusPublisher;

        public UsersController(UsersDbContext context, EventBusPublisher eventBusPublisher)
        {
            _context = context;
            _eventBusPublisher = eventBusPublisher;
        }


        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Users.ToList());


        /*
        //METODO CON HTTP
        [HttpPost]
        public IActionResult Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = user.Id }, user);
        }*/


        // METODO CON RabbitMQ
        [HttpPost]
        public IActionResult Create(User user)
        {
            // Agregar el usuario a la base de datos
            _context.Users.Add(user);
            _context.SaveChanges();

            // Publicar el evento a RabbitMQ
            var userCreatedEvent = new
            {
                user.Id,
                user.Nombre,
                user.Email
            };

            // Enviar el evento a RabbitMQ
            _eventBusPublisher.PublishUserCreated(userCreatedEvent);

            // Devolver la respuesta adecuada
            return CreatedAtAction(nameof(GetAll), new { id = user.Id }, user);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return user;
        }
    }
}
