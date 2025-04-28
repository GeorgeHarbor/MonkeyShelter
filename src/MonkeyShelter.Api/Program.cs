using System.Security.Claims;

using Carter;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Api.Extensions;
using MonkeyShelter.Application;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Infrastructure;
using MonkeyShelter.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(opt => opt.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigins",
        policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()));
builder.Services
    .AddScoped(typeof(IRepository<>), typeof(Repository<>))
    .AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IMonkeyService, MonkeyService>();
builder.Services.AddScoped<IMonkeyRepository, MonkeyRepository>();
builder.Services.AddScoped<IManagerShelterRepository, ManagerShelterRepository>();

builder.Services.AddCarter();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddMessaging(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MonkeyShelter.Api"));
}
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    await db.SeedAsync();
}

app.Run();