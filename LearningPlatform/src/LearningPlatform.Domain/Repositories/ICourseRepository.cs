using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Domain.Repositories;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<IReadOnlyList<Course>> GetAllWithCategoryAsync(int? categoryId);
}
