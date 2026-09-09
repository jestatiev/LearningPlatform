using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- M:N User <-> Role (kompozitni ključ) ---
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // --- M:N User <-> Course preko Enrollment (join entitet s payloadom) ---
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique(); // korisnik se ne može dvaput prijaviti na isti tečaj

        // --- 1:N Category -> Course ---
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- 1:N Course -> Lesson ---
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- 1:N Course -> Test -> Question ---
        modelBuilder.Entity<Test>()
            .HasOne(t => t.Course)
            .WithMany(c => c.Tests)
            .HasForeignKey(t => t.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Test)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- 1:N Course -> Review, User -> Review ---
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Course)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();

        modelBuilder.Entity<Course>().Property(c => c.Price).HasColumnType("decimal(10,2)");

        base.OnModelCreating(modelBuilder);
    }
}
