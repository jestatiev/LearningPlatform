namespace LearningPlatform.Application.DTOs;

public record CategoryDto(int Id, string Name, string? Description, int CourseCount);

public record CreateCategoryDto(string Name, string? Description);
