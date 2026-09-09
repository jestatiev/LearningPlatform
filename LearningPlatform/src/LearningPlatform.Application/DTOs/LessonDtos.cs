namespace LearningPlatform.Application.DTOs;

public record LessonDto(int Id, string Title, string Content, string? VideoUrl, int OrderIndex, int CourseId);

public record CreateLessonDto(string Title, string Content, string? VideoUrl, int OrderIndex, int CourseId);

public record UpdateLessonDto(string Title, string Content, string? VideoUrl, int OrderIndex);
