using Carter;

using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Api.Extensions;
using MonkeyShelter.Application;
using MonkeyShelter.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(opt => opt.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services
    .AddScoped(typeof(IRepository<>), typeof(Repository<>))
    .AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddCarter();
builder.Services.AddMessaging(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "swagger"));
}

app.UseHttpsRedirection();

app.MapCarter();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    await db.SeedAsync();
}

app.Run();