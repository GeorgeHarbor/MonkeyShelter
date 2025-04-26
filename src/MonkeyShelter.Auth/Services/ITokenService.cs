using MonkeyShelter.Auth.Models;

namespace MonkeyShelter.Auth.Services;

public interface ITokenService
{
    string CreateToken(User user);
}