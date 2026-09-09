using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class TestsControllerTests
{
    private readonly Mock<ITestService> _serviceMock = new();
    private readonly TestsController _controller;

    public TestsControllerTests()
    {
        _controller = new TestsController(_serviceMock.Object);
        TestHelpers.SetUser(_controller, userId: 5, role: "Student");
    }

    [Fact]
    public async Task Submit_ReturnsOk_WithResult()
    {
        var dto = new SubmitTestDto(new List<SubmitAnswerDto> { new(1, 'A') });
        var expected = new TestResultDto(1, 1, 100.0);
        _serviceMock.Setup(s => s.SubmitAsync(1, dto)).ReturnsAsync(expected);

        var result = await _controller.Submit(1, dto);

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AddQuestion_ReturnsOk_WithCreatedQuestion()
    {
        var dto = new CreateQuestionDto("Što je EF Core?", "ORM", "IDE", "Baza", "Jezik", 'A', 1);
        var expected = new QuestionAdminDto(1, dto.Text, dto.OptionA, dto.OptionB, dto.OptionC, dto.OptionD, 'A', 1);
        _serviceMock.Setup(s => s.AddQuestionAsync(dto)).ReturnsAsync(expected);

        var result = await _controller.AddQuestion(dto);

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expected);
    }
}
