using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class CoursesControllerTests
{
    private readonly Mock<ICourseService> _serviceMock = new();
    private readonly CoursesController _controller;

    public CoursesControllerTests()
    {
        _controller = new CoursesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithCourses()
    {
        var courses = new List<CourseListItemDto> { new(1, "C# osnove", "Ivan Ivić", 49.99m, "Programiranje", 4.5) };
        _serviceMock.Setup(s => s.GetAllAsync(null)).ReturnsAsync(courses);

        var result = await _controller.GetAll(null);

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(courses);
    }

    [Fact]
    public async Task GetById_WhenNotFound_PropagatesNotFoundException()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ThrowsAsync(new NotFoundException("Course", 99));

        Func<Task> act = async () => await _controller.GetById(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        var dto = new UpdateCourseDto("Naslov", "Opis", "Predavač", 10m, 1);
        _serviceMock.Setup(s => s.UpdateAsync(1, dto)).Returns(Task.CompletedTask);

        var result = await _controller.Update(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }
}
