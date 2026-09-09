namespace LearningPlatform.Application.DTOs;

public record CourseListItemDto(int Id, string Title, string InstructorName, decimal Price, string CategoryName, double AverageRating);

public record CourseDetailsDto(
    int Id,
    string Title,
    string Description,
    string InstructorName,
    decimal Price,
    DateTime CreatedAt,
    string CategoryName,
    int CategoryId,
    List<LessonDto> Lessons,
    List<ReviewDto> Reviews,
    double AverageRating,
    int EnrollmentCount);

public record CreateCourseDto(string Title, string Description, string InstructorName, decimal Price, int CategoryId);

public record UpdateCourseDto(string Title, string Description, string InstructorName, decimal Price, int CategoryId);
