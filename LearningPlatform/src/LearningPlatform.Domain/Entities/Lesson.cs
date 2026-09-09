namespace LearningPlatform.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int OrderIndex { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
