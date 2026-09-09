using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
public class LessonsController : BaseApiController
{
    private readonly ILessonService _service;

    public LessonsController(ILessonService service) => _service = service;

    [HttpGet("by-course/{courseId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<LessonDto>>> GetByCourse(int courseId) =>
        Ok(await _service.GetByCourseAsync(courseId));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<LessonDto>> GetById(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<LessonDto>> Create(CreateLessonDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, UpdateLessonDto dto)
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
