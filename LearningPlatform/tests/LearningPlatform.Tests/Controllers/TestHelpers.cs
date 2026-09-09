using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Tests.Controllers;

public static class TestHelpers
{
    public static void SetUser(ControllerBase controller, int userId, string role)
    {
        var claims = new List<Claim>
        {
            new("uid", userId.ToString()),
            new(ClaimTypes.Name, $"user{userId}"),
            new(ClaimTypes.Role, role),
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
