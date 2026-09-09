namespace LearningPlatform.Domain.Entities;

public class Question : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public char CorrectOption { get; set; } // 'A' | 'B' | 'C' | 'D'

    public int TestId { get; set; }
    public Test Test { get; set; } = null!;
}
