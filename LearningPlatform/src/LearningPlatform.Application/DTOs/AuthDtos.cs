namespace LearningPlatform.Application.DTOs;

public record RegisterDto(string Username, string Email, string Password, string FullName);

public record LoginDto(string Username, string Password);

public record AuthResponseDto(string Token, DateTime ExpiresAt, string Username, List<string> Roles);
