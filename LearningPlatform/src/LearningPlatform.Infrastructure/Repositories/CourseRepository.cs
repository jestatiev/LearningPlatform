using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using LearningPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext context) : base(context) { }

    public async Task<Course?> GetByIdWithDetailsAsync(int id) =>
        await DbSet
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Tests)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IReadOnlyList<Course>> GetAllWithCategoryAsync(int? categoryId) =>
        await DbSet
            .Include(c => c.Category)
            .Where(c => categoryId == null || c.CategoryId == categoryId)
            .ToListAsync();
}
