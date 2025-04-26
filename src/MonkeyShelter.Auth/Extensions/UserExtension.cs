using MonkeyShelter.Auth.Models;
using MonkeyShelter.Auth.Models.Dtos;

namespace MonkeyShelter.Auth.Extensions;

public static class UserExtension
{
    public static UserDto MapToDto(this User user)
    {
        return new UserDto
        {
            UserName = user.UserName!,
            Email = user.Email!,
            Id = user.Id,
        };
    }
}