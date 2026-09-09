using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class EnrollmentsControllerTests
{
    private readonly Mock<IEnrollmentService> _serviceMock = new();
    private readonly EnrollmentsController _controller;

    public EnrollmentsControllerTests()
    {
        _controller = new EnrollmentsController(_serviceMock.Object);
        TestHelpers.SetUser(_controller, userId: 5, role: "Student");
    }

    [Fact]
    public async Task GetMy_ReturnsOk_WithEnrollmentsForCurrentUser()
    {
        var enrollments = new List<EnrollmentDto> { new(1, 10, "C# osnove", DateTime.UtcNow, 40, false) };
        _serviceMock.Setup(s => s.GetMyEnrollmentsAsync(5)).ReturnsAsync(enrollments);

        var result = await _controller.GetMy();

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(enrollments);
    }

    [Fact]
    public async Task Enroll_ReturnsOk_UsingCurrentUserId()
    {
        var dto = new CreateEnrollmentDto(10);
        var enrollment = new EnrollmentDto(1, 10, "C# osnove", DateTime.UtcNow, 0, false);
        _serviceMock.Setup(s => s.EnrollAsync(5, dto)).ReturnsAsync(enrollment);

        var result = await _controller.Enroll(dto);

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(enrollment);
        _serviceMock.Verify(s => s.EnrollAsync(5, dto), Times.Once);
    }

    [Fact]
    public async Task Unenroll_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.UnenrollAsync(5, 1)).Returns(Task.CompletedTask);

        var result = await _controller.Unenroll(1);

        result.Should().BeOfType<NoContentResult>();
    }
}
