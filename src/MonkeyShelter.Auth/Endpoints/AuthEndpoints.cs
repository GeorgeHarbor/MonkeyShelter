
using MassTransit;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using MonkeyShelter.Application.Events;
using MonkeyShelter.Auth.Data;
using MonkeyShelter.Auth.Extensions;
using MonkeyShelter.Auth.Models;
using MonkeyShelter.Auth.Models.Dtos;
using MonkeyShelter.Auth.Services;

namespace MonkeyShelter.Auth.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoint(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/auth").WithTags("Auth");

        grp.MapGet("/users/{id}", GetUserById)
            .WithName("GetUserById")
            .Produces<UserDto>(200)
            .Produces(404);

        grp.MapPost("/register", Register)
            .WithName("Register")
            .Produces<UserDto>(201)
            .Produces<ValidationProblemDetails>(401);

        grp.MapPost("/login", Login)
            .WithName("Login")
            .Produces<string>(200)
            .Produces(401);
    }

    #region Methods
    private static async Task<IResult> GetUserById(
            string id,
            UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(id);
        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(user.MapToDto());
    }

    private static async Task<IResult> Login(LoginRequest req,
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    IConfiguration config,
    ITokenService tokenService,
    ILogger<Program> logger)
    {
        logger.LogInformation("Login requested for {Username}", req.Username);
        var signInResult = await signInManager
            .PasswordSignInAsync(req.Username, req.Password, isPersistent: false, lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {

            logger.LogWarning("Login FAILED for {Username}", req.Username);
            return TypedResults.Unauthorized();
        }

        var user = await userManager.FindByNameAsync(req.Username)!;
        string jwt = tokenService.CreateToken(user!);

        logger.LogInformation("Login SUCCEEDED for {Username}", req.Username);

        UserDto dto = new()
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user!.Email!,
            Token = jwt
        };


        return TypedResults.Ok(dto);
    }

    private static async Task<IResult> Register(
        RegisterRequest req,
        DataContext db,
        UserManager<User> userManager,
        ILogger<Program> logger,
        IPublishEndpoint publisher)
    {
        logger.LogInformation("Registering {Username}", req.Username);
        var user = new User { UserName = req.Username, Email = req.Email };
        var result = await userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            logger.LogWarning("Registration failed for {Username}: {Errors}",
                 req.Username,
                 result.Errors.Select(e => e.Description));
            return TypedResults.ValidationProblem(
                result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
            );
        }

        logger.LogError("RADI");
        await publisher.Publish(new UserRegistered(
                    Guid.Parse(user.Id),
                    user.Email,
                    Guid.Parse(req.ShelterId),
                    DateTime.Now
                    ), ctx => ctx.Headers.Set("MT-Message-Name", nameof(UserRegistered)));

        return Results.CreatedAtRoute(
          "GetUserById",
          new { id = user.Id },
          user.MapToDto()
        );
    }
    #endregion
}