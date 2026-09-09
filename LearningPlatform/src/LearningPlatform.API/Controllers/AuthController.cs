using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto) =>
        Ok(await _authService.RegisterAsync(dto));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto) =>
        Ok(await _authService.LoginAsync(dto));
}
