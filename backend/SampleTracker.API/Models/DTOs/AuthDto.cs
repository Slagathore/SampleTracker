namespace SampleTracker.API.Models.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string DisplayName, string Role);
public record RegisterRequest(string Email, string Password, string DisplayName);
