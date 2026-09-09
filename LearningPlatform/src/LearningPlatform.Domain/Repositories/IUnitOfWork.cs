using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Domain.Repositories;

// Unit of Work - grupira sve repozitorije i osigurava jedan SaveChanges (jedna transakcija)
public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ICourseRepository Courses { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Lesson> Lessons { get; }
    IGenericRepository<Enrollment> Enrollments { get; }
    IGenericRepository<Test> Tests { get; }
    IGenericRepository<Question> Questions { get; }
    IGenericRepository<Review> Reviews { get; }

    Task<int> SaveChangesAsync();
}
