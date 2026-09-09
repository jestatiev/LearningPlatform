using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _serviceMock = new();
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _controller = new UsersController(_serviceMock.Object);
        TestHelpers.SetUser(_controller, userId: 3, role: "Admin");
    }

    [Fact]
    public async Task GetMe_ReturnsOk_ForCurrentUser()
    {
        var expected = new UserDto(3, "admin", "admin@test.hr", "Administrator", DateTime.UtcNow, new List<string> { "Admin" });
        _serviceMock.Setup(s => s.GetByIdAsync(3)).ReturnsAsync(expected);

        var result = await _controller.GetMe();

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithAllUsers()
    {
        var users = new List<UserDto> { new(1, "marko", "marko@test.hr", "Marko Marić", DateTime.UtcNow, new List<string> { "Student" }) };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

        var result = await _controller.GetAll();

        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(users);
    }
}
