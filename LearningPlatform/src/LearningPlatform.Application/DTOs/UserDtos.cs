namespace LearningPlatform.Application.DTOs;

public record UserDto(int Id, string Username, string Email, string FullName, DateTime CreatedAt, List<string> Roles);
