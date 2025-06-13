using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsService.Data;
using ProductsService.Models;
using ProductsService.Services;

namespace ProductsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetById(id);
            if(product == null) 
                return NotFound();
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAll();
            return Ok(products);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var productCreated = await _productService.Create(product);
            return CreatedAtAction(nameof(GetById), new { id = productCreated.Id }, productCreated);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetProductsByUserId(int userId)
        {
            var products = await _productService.GetProductsByUserId(userId);
            return Ok(products);
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
