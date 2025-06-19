using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersService.Data;
using UsersService.DTO;
using UsersService.Messaging;
using UsersService.Models;
using UsersService.Services;

namespace UsersService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            var userCreated = _userService.Create(user);

            // Devolver la respuesta adecuada
            return CreatedAtAction(nameof(GetAll), new { id = userCreated.Id }, userCreated);
        }

        [HttpGet("with-products")]
        public async Task<IActionResult> GetUsersWithProducts()
        {
            var usersDTO = await _userService.GetUsersWithProducts();
            return Ok(usersDTO);
        }

    }
}
