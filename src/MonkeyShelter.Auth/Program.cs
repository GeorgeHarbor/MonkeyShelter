using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MonkeyShelter.Auth.Data;
using MonkeyShelter.Auth.Endpoints;
using MonkeyShelter.Auth.Models;
using MonkeyShelter.Auth.Services;

using Serilog;
using MonkeyShelter.Auth;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.Configure<JwtSettings>(
        builder.Configuration.GetSection("Jwt")
        );

builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigins",
        policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()));
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Key));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
        .AddJwtBearer(opt =>
        {
            opt.RequireHttpsMetadata = true;
            opt.SaveToken = true;
            opt.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtKey,
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ClockSkew = TimeSpan.Zero
            };
        });
builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ));

builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseExceptionHandler(errApp =>
    errApp.Run(async ctx =>
    {
        var ex = ctx.Features
                    .Get<IExceptionHandlerFeature>()?
                    .Error;

        await Results.Problem(
            title: "Internal Server Error",
            detail: ex?.Message,
            statusCode: 500,
            instance: ctx.Request.Path
        ).ExecuteAsync(ctx);
    })
);
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();


app.MapAuthEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}")
        .WithOpenApi();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1", "MonkeyShelter Auth API"));
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}
app.Run();