using Microsoft.AspNetCore.Mvc;

namespace OSN.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private const string EntityName = "auth";

    [HttpGet($"/{EntityName}/generate-token")]
    public async Task<IActionResult> GenerateToken()
    {
        return Ok("Test endpoint");
    }
}
