using Microsoft.EntityFrameworkCore;
using UsersService.Data;

var builder = WebApplication.CreateBuilder(args);

// Base de datos
builder.Services.AddDbContext<UsersDbContext>(opt =>
    opt.UseSqlite("Data Source=users.db"));

// Comunicación con ProductsService
builder.Services.AddHttpClient("ProductsAPI", client =>
{
    client.BaseAddress = new Uri("http://products-service:8080/");
});

// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

