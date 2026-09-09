using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
public class CategoriesController : BaseApiController
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDto>> GetById(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, CreateCategoryDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
