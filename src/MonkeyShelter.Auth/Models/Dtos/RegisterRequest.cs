namespace MonkeyShelter.Auth.Models.Dtos;

public record RegisterRequest(string Username, string Email, string ShelterId, string Password);