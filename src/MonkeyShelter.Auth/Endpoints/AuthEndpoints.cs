
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using MonkeyShelter.Auth.Data;
using MonkeyShelter.Auth.Models;
using MonkeyShelter.Auth.Models.Dtos;

namespace MonkeyShelter.Auth.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", Register);
        app.MapPost("/login", Login);
    }

    private static async Task<IResult> Login(LoginRequest req,
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    IConfiguration config)
    {
        var signInResult = await signInManager
            .PasswordSignInAsync(req.Username, req.Password, isPersistent: false, lockoutOnFailure: false);

        if (!signInResult.Succeeded)
            return Results.Unauthorized();

        // 2. Retrieve the user & roles
        var user = await userManager.FindByNameAsync(req.Username)!;
        var roles = await userManager.GetRolesAsync(user!);

        // 3. Build claims for the token
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user!.Id),
        new(ClaimTypes.Name, user.UserName!),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // 4. Generate JWT
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return TypedResults.Ok(jwt);
    }

    private static async Task<IResult> Register(RegisterRequest req, DataContext db, UserManager<User> userManager)
    {
        var user = new User { UserName = req.Username, Email = req.Email };
        var result = await userManager.CreateAsync(user, req.Password);

        return !result.Succeeded ? TypedResults.BadRequest(result.Errors) : TypedResults.Created();
    }
}