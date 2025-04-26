using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MonkeyShelter.Auth.Data;
using MonkeyShelter.Auth.Endpoints;
using MonkeyShelter.Auth.Models;
using MonkeyShelter.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(
        builder.Configuration.GetSection("Jwt")
        );

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
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ));

builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

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