using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using MonkeyShelter.Auth.Models;

namespace MonkeyShelter.Auth.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _opts;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IOptions<JwtSettings> opts)
    {
        _opts = opts.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!)
        };

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                issuer: _opts.Issuer,
                audience: _opts.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_opts.ExpiresInMinutes),
                signingCredentials: credentials
                );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}