using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
public class ReviewsController : BaseApiController
{
    private readonly IReviewService _service;

    public ReviewsController(IReviewService service) => _service = service;

    [HttpGet("by-course/{courseId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetByCourse(int courseId) =>
        Ok(await _service.GetByCourseAsync(courseId));

    [HttpPost]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<ReviewDto>> Create(CreateReviewDto dto) =>
        Ok(await _service.CreateAsync(CurrentUserId, dto));

    [HttpPut("{id:int}")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> Update(int id, UpdateReviewDto dto)
    {
        await _service.UpdateAsync(CurrentUserId, id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(CurrentUserId, IsAdmin, id);
        return NoContent();
    }
}
