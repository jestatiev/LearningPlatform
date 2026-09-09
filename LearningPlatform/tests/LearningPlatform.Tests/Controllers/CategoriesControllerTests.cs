using FluentAssertions;
using LearningPlatform.API.Controllers;
using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LearningPlatform.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _serviceMock = new();
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _controller = new CategoriesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithListOfCategories()
    {
        var categories = new List<CategoryDto> { new(1, "Programiranje", "Opis", 5) };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(categories);

        var result = await _controller.GetAll();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(categories);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var dto = new CreateCategoryDto("Dizajn", "Opis dizajna");
        var created = new CategoryDto(2, "Dizajn", "Opis dizajna", 0);
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }
}
