using LearningPlatform.Application.DTOs;

namespace LearningPlatform.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task UpdateAsync(int id, CreateCategoryDto dto);
    Task DeleteAsync(int id);
}

public interface ICourseService
{
    Task<List<CourseListItemDto>> GetAllAsync(int? categoryId);
    Task<CourseDetailsDto> GetByIdAsync(int id);
    Task<CourseDetailsDto> CreateAsync(CreateCourseDto dto);
    Task UpdateAsync(int id, UpdateCourseDto dto);
    Task DeleteAsync(int id);
}

public interface ILessonService
{
    Task<List<LessonDto>> GetByCourseAsync(int courseId);
    Task<LessonDto> GetByIdAsync(int id);
    Task<LessonDto> CreateAsync(CreateLessonDto dto);
    Task UpdateAsync(int id, UpdateLessonDto dto);
    Task DeleteAsync(int id);
}

public interface IEnrollmentService
{
    Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(int userId);
    Task<EnrollmentDto> EnrollAsync(int userId, CreateEnrollmentDto dto);
    Task UpdateProgressAsync(int userId, int enrollmentId, UpdateProgressDto dto);
    Task UnenrollAsync(int userId, int enrollmentId);
    Task<List<EnrollmentDto>> GetByCourseAsync(int courseId);
}

public interface ITestService
{
    Task<List<TestDto>> GetByCourseAsync(int courseId);
    Task<TestDto> GetByIdAsync(int id);
    Task<TestDto> CreateAsync(CreateTestDto dto);
    Task DeleteAsync(int id);
    Task<QuestionAdminDto> AddQuestionAsync(CreateQuestionDto dto);
    Task DeleteQuestionAsync(int questionId);
    Task<TestResultDto> SubmitAsync(int testId, SubmitTestDto dto);
}

public interface IReviewService
{
    Task<List<ReviewDto>> GetByCourseAsync(int courseId);
    Task<ReviewDto> CreateAsync(int userId, CreateReviewDto dto);
    Task UpdateAsync(int userId, int reviewId, UpdateReviewDto dto);
    Task DeleteAsync(int userId, bool isAdmin, int reviewId);
}

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
}
