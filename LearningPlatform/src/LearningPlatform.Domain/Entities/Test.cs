namespace LearningPlatform.Domain.Entities;

public class Test : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
