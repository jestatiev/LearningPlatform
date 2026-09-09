using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ReturnsOk_WithAuthResponse()
    {
        var dto = new RegisterDto("marko", "marko@test.hr", "Lozinka123!", "Marko Marić");
        var expected = new AuthResponseDto("token123", DateTime.UtcNow.AddHours(2), "marko", new List<string> { "Student" });
        _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(expected);

        var result = await _controller.Register(dto);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Login_ReturnsOk_WithAuthResponse()
    {
        var dto = new LoginDto("marko", "Lozinka123!");
        var expected = new AuthResponseDto("token123", DateTime.UtcNow.AddHours(2), "marko", new List<string> { "Student" });
        _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(expected);

        var result = await _controller.Login(dto);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expected);
    }
}
