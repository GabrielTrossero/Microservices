using Microsoft.EntityFrameworkCore;
using UsersService.Data;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<UsersDbContext>(opt =>
    opt.UseSqlite("Data Source=users.db"));

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

