namespace LearningPlatform.Application.DTOs;

public record QuestionDto(int Id, string Text, string OptionA, string OptionB, string OptionC, string OptionD);

// Verzija s točnim odgovorom - koristi se samo kod administracije, ne kod polaganja testa
public record QuestionAdminDto(int Id, string Text, string OptionA, string OptionB, string OptionC, string OptionD, char CorrectOption, int TestId);

public record CreateQuestionDto(string Text, string OptionA, string OptionB, string OptionC, string OptionD, char CorrectOption, int TestId);

public record TestDto(int Id, string Title, int CourseId, List<QuestionDto> Questions);

public record CreateTestDto(string Title, int CourseId);

public record SubmitAnswerDto(int QuestionId, char SelectedOption);

public record SubmitTestDto(List<SubmitAnswerDto> Answers);

public record TestResultDto(int TotalQuestions, int CorrectAnswers, double ScorePercent);
