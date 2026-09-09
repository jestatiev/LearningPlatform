namespace LearningPlatform.Domain.Entities;

// M:N relacija User <-> Course, s dodatnim atributima (napredak, datum prijave)
public class Enrollment : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public int ProgressPercent { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
}
