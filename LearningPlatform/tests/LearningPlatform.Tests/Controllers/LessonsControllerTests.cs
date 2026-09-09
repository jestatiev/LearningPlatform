using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class LessonsControllerTests
{
    private readonly Mock<ILessonService> _serviceMock = new();
    private readonly LessonsController _controller;

    public LessonsControllerTests()
    {
        _controller = new LessonsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetByCourse_ReturnsOk_WithLessons()
    {
        var lessons = new List<LessonDto> { new(1, "Uvod", "Sadržaj", null, 1, 1) };
        _serviceMock.Setup(s => s.GetByCourseAsync(1)).ReturnsAsync(lessons);

        var result = await _controller.GetByCourse(1);

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(lessons);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var dto = new CreateLessonDto("Uvod", "Sadržaj", null, 1, 1);
        var created = new LessonDto(1, "Uvod", "Sadržaj", null, 1, 1);
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(created);
    }
}
