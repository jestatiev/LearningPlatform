using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : BaseApiController
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service) => _service = service;

    [HttpGet("my")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetMy() =>
        Ok(await _service.GetMyEnrollmentsAsync(CurrentUserId));

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Enroll(CreateEnrollmentDto dto) =>
        Ok(await _service.EnrollAsync(CurrentUserId, dto));

    [HttpPut("{id:int}/progress")]
    public async Task<IActionResult> UpdateProgress(int id, UpdateProgressDto dto)
    {
        await _service.UpdateProgressAsync(CurrentUserId, id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Unenroll(int id)
    {
        await _service.UnenrollAsync(CurrentUserId, id);
        return NoContent();
    }

    [HttpGet("by-course/{courseId:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetByCourse(int courseId) =>
        Ok(await _service.GetByCourseAsync(courseId));
}
