namespace LearningPlatform.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty; // "Admin" ili "Student"

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
