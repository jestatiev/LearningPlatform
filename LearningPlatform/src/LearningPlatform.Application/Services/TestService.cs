using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Services;

public class TestService : ITestService
{
    private readonly IUnitOfWork _uow;

    public TestService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<TestDto>> GetByCourseAsync(int courseId)
    {
        var tests = await _uow.Tests.Query()
            .Include(t => t.Questions)
            .Where(t => t.CourseId == courseId)
            .ToListAsync();

        return tests.Select(MapToDto).ToList();
    }

    public async Task<TestDto> GetByIdAsync(int id)
    {
        var test = await _uow.Tests.Query()
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException(nameof(Test), id);

        return MapToDto(test);
    }

    public async Task<TestDto> CreateAsync(CreateTestDto dto)
    {
        _ = await _uow.Courses.GetByIdAsync(dto.CourseId) ?? throw new BadRequestException("Tečaj ne postoji.");

        var test = new Test { Title = dto.Title, CourseId = dto.CourseId };
        await _uow.Tests.AddAsync(test);
        await _uow.SaveChangesAsync();
        return MapToDto(test);
    }

    public async Task DeleteAsync(int id)
    {
        var test = await _uow.Tests.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Test), id);
        _uow.Tests.Remove(test);
        await _uow.SaveChangesAsync();
    }

    public async Task<QuestionAdminDto> AddQuestionAsync(CreateQuestionDto dto)
    {
        _ = await _uow.Tests.GetByIdAsync(dto.TestId) ?? throw new BadRequestException("Test ne postoji.");

        var question = new Question
        {
            Text = dto.Text,
            OptionA = dto.OptionA,
            OptionB = dto.OptionB,
            OptionC = dto.OptionC,
            OptionD = dto.OptionD,
            CorrectOption = char.ToUpperInvariant(dto.CorrectOption),
            TestId = dto.TestId,
        };
        await _uow.Questions.AddAsync(question);
        await _uow.SaveChangesAsync();

        return new QuestionAdminDto(question.Id, question.Text, question.OptionA, question.OptionB,
            question.OptionC, question.OptionD, question.CorrectOption, question.TestId);
    }

    public async Task DeleteQuestionAsync(int questionId)
    {
        var question = await _uow.Questions.GetByIdAsync(questionId) ?? throw new NotFoundException(nameof(Question), questionId);
        _uow.Questions.Remove(question);
        await _uow.SaveChangesAsync();
    }

    public async Task<TestResultDto> SubmitAsync(int testId, SubmitTestDto dto)
    {
        var test = await _uow.Tests.Query()
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.Id == testId)
            ?? throw new NotFoundException(nameof(Test), testId);

        int correct = 0;
        foreach (var answer in dto.Answers)
        {
            var question = test.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question != null && char.ToUpperInvariant(answer.SelectedOption) == question.CorrectOption)
                correct++;
        }

        var total = test.Questions.Count;
        var scorePercent = total == 0 ? 0 : Math.Round(correct * 100.0 / total, 2);
        return new TestResultDto(total, correct, scorePercent);
    }

    private static TestDto MapToDto(Test t) => new(
        t.Id, t.Title, t.CourseId,
        t.Questions.Select(q => new QuestionDto(q.Id, q.Text, q.OptionA, q.OptionB, q.OptionC, q.OptionD)).ToList());
}
