using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
public class CoursesController : BaseApiController
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CourseListItemDto>>> GetAll([FromQuery] int? categoryId) =>
        Ok(await _service.GetAllAsync(categoryId));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseDetailsDto>> GetById(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CourseDetailsDto>> Create(CreateCourseDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
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
