namespace LearningPlatform.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Test> Tests { get; set; } = new List<Test>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    // M:N (preko Enrollment koji nosi dodatne podatke) -> Course <-> User
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
