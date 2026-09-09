using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using LearningPlatform.Infrastructure.Data;

namespace LearningPlatform.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Courses = new CourseRepository(_context);
        Roles = new GenericRepository<Role>(_context);
        Categories = new GenericRepository<Category>(_context);
        Lessons = new GenericRepository<Lesson>(_context);
        Enrollments = new GenericRepository<Enrollment>(_context);
        Tests = new GenericRepository<Test>(_context);
        Questions = new GenericRepository<Question>(_context);
        Reviews = new GenericRepository<Review>(_context);
    }

    public IUserRepository Users { get; }
    public ICourseRepository Courses { get; }
    public IGenericRepository<Role> Roles { get; }
    public IGenericRepository<Category> Categories { get; }
    public IGenericRepository<Lesson> Lessons { get; }
    public IGenericRepository<Enrollment> Enrollments { get; }
    public IGenericRepository<Test> Tests { get; }
    public IGenericRepository<Question> Questions { get; }
    public IGenericRepository<Review> Reviews { get; }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
