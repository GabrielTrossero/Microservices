using Microsoft.AspNetCore.Mvc;
using ProductsService.Data;
using ProductsService.Models;

namespace ProductsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsDbContext _context;

        public ProductsController(ProductsDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Products.ToList());


        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsersFromUserService([FromServices] IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient("UsersAPI");

            try
            {
                var response = await client.GetAsync("api/users");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error comunicando con UsersService: {ex.Message}");
            }
        }

        [HttpGet("usuarios/{id}")]
        public async Task<IActionResult> GetUserByIdFromUsersService(int id, [FromServices] IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient("UsersAPI");

            try
            {
                var response = await client.GetAsync($"api/users/{id}");
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "No se encontró el usuario.");

                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error comunicando con ProductsService: {ex.Message}");
            }
        }
    }
}
