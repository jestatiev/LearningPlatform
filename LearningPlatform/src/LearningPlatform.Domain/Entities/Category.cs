namespace LearningPlatform.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // 1:N -> jedna kategorija ima mnogo tečajeva
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
