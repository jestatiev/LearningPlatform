using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
public class TestsController : BaseApiController
{
    private readonly ITestService _service;

    public TestsController(ITestService service) => _service = service;

    [HttpGet("by-course/{courseId:int}")]
    [Authorize]
    public async Task<ActionResult<List<TestDto>>> GetByCourse(int courseId) =>
        Ok(await _service.GetByCourseAsync(courseId));

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<TestDto>> GetById(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<TestDto>> Create(CreateTestDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("questions")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<QuestionAdminDto>> AddQuestion(CreateQuestionDto dto) =>
        Ok(await _service.AddQuestionAsync(dto));

    [HttpDelete("questions/{questionId:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteQuestion(int questionId)
    {
        await _service.DeleteQuestionAsync(questionId);
        return NoContent();
    }

    [HttpPost("{id:int}/submit")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<TestResultDto>> Submit(int id, SubmitTestDto dto) =>
        Ok(await _service.SubmitAsync(id, dto));
}
