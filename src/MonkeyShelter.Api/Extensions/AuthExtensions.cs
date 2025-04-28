using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using MonkeyShelter.Application.Interfaces;

namespace MonkeyShelter.Api.Extensions;

public static class AuthExtensions
{

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"]!;
        var jwtIssuer = configuration["Jwt:Issuer"]!;
        var jwtAudience = configuration["Jwt:Audience"]!;


        services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
        .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                            ),
                        ClockSkew = TimeSpan.Zero
                    };
                });


        services.AddAuthorizationBuilder()
            .AddPolicy("ShelterManager", policy =>
                    policy.RequireAssertion(context =>
                        {
                            // 1) Grab the HttpContext so we can see the route
                            var http = (context.Resource as HttpContext)!;
                            var rv = http.GetRouteData().Values;

                            // 2) Pull the shelterId from the URL
                            if (!rv.TryGetValue("shelterId", out var raw) ||
                                    raw is not string rawId ||
                                    !Guid.TryParse(rawId, out var shelterId))
                                return false;

                            // 3) Pull the userId (sub/NameIdentifier) from the JWT
                            var sub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            if (!Guid.TryParse(sub, out var userId))
                                return false;

                            // 4) Resolve your repo & check the table synchronously
                            var repo = http.RequestServices
                                .GetRequiredService<IManagerShelterRepository>();
                            return repo
                                .IsManagerOfShelterAsync(userId, shelterId)
                                .GetAwaiter()
                                .GetResult();
                        })
        );
        services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "MonkeyShelter API",
                        Version = "v1"
                    });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.\n\n" +
                            "Enter your token like: Bearer {your JWT}",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                        {
                        new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name   = "Bearer",
                        In     = ParameterLocation.Header
                        },
                        new List<string>()
                        }
                        });
                });
        return services;
    }
}