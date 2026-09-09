using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class ReviewsControllerTests
{
    private readonly Mock<IReviewService> _serviceMock = new();
    private readonly ReviewsController _controller;

    public ReviewsControllerTests()
    {
        _controller = new ReviewsController(_serviceMock.Object);
        TestHelpers.SetUser(_controller, userId: 7, role: "Student");
    }

    [Fact]
    public async Task Create_ReturnsOk_UsingCurrentUserId()
    {
        var dto = new CreateReviewDto(5, "Odličan tečaj", 1);
        var expected = new ReviewDto(1, 5, "Odličan tečaj", DateTime.UtcNow, "user7", 1);
        _serviceMock.Setup(s => s.CreateAsync(7, dto)).ReturnsAsync(expected);

        var result = await _controller.Create(dto);

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(7, false, 1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
        _serviceMock.Verify(s => s.DeleteAsync(7, false, 1), Times.Once);
    }
}
