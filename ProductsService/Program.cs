using Microsoft.EntityFrameworkCore;
using ProductsService.Data;
using ProductsService.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Base de datos
builder.Services.AddDbContext<ProductsDbContext>(opt =>
    opt.UseSqlite("Data Source=products.db"));

// Comunicación con UsersService
builder.Services.AddHttpClient("UsersAPI", client =>
{
    client.BaseAddress = new Uri("http://users-service:8080/");
});

// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<EventBusConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Iniciar el consumidor de eventos
var eventBusConsumer = app.Services.GetRequiredService<EventBusConsumer>();

app.MapControllers();
app.Run();
