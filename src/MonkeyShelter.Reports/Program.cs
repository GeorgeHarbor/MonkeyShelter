using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Api.Extensions;
using MonkeyShelter.Application;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Infrastructure;
using MonkeyShelter.Infrastructure.Repositories;
using MonkeyShelter.Reports;
using MonkeyShelter.Reports.Services;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ReportService:";
});
builder.Services.AddScoped<ReportService>();
builder.Services.AddDbContext<DataContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMonkeyRepository, MonkeyRepository>();
builder.Services.AddScoped<IArrivalRepository, ArrivalsRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigins",
        policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!
    ));

builder.Services.AddSingleton<IReportCacheInvalidator, ReportCacheInvalidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "swagger"));
}
app.MapGet("/reports/count-per-species", async (ReportService svc) =>
{
    var data = await svc.GetCountPerSpeciesAsync();
    return Results.Ok(data);
});

// 5) Endpoint: arrivals in range
//    e.g. GET /reports/arrivals?from=2025-04-01&to=2025-04-15
app.MapGet("/reports/arrivals", async (
        ReportService svc,
        DateTime from,
        DateTime to) =>
{
    if (from > to)
        return Results.BadRequest("'from' must be <= 'to'");
    var data = await svc.GetArrivalsInRangeAsync(from, to);
    return Results.Ok(data);
});
app.UseCors("AllowSpecificOrigins");
app.UseHttpsRedirection();

app.Run();