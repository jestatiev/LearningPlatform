namespace LearningPlatform.Application.DTOs;

public record EnrollmentDto(int Id, int CourseId, string CourseTitle, DateTime EnrolledAt, int ProgressPercent, bool IsCompleted);

public record CreateEnrollmentDto(int CourseId);

public record UpdateProgressDto(int ProgressPercent, bool IsCompleted);
