using DroneRental.Infrastructure.Data;
using DroneRental.Api.Services.Rentals;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IRentalPricingService, RentalPricingService>();

// Registration database in SQLite in .NET system
builder.Services.AddDbContext<DroneRentalDbContext>(options =>
    options.UseSqlite("Data Source=../DroneRental.Api/DroneRental.db"));

// Swagger registration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Drone Rental API v1");
    });
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

