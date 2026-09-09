using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<UserDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe() => Ok(await _service.GetByIdAsync(CurrentUserId));

    [HttpGet("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<UserDto>> GetById(int id) => Ok(await _service.GetByIdAsync(id));
}
