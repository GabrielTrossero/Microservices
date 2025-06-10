using Microsoft.EntityFrameworkCore;
using UsersService.Data;
using UsersService.Messaging;

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
builder.Services.AddSingleton<EventBusPublisher>();

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

