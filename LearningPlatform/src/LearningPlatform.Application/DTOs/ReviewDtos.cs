namespace LearningPlatform.Application.DTOs;

public record ReviewDto(int Id, int Rating, string? Comment, DateTime CreatedAt, string Username, int CourseId);

public record CreateReviewDto(int Rating, string? Comment, int CourseId);

public record UpdateReviewDto(int Rating, string? Comment);
