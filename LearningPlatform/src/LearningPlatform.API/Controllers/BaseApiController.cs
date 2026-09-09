using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected int CurrentUserId =>
        int.Parse(User.Claims.First(c => c.Type == "uid").Value);

    protected bool IsAdmin => User.IsInRole("Admin");
}
