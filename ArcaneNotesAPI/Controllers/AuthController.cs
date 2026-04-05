using ArcaneNotesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArcaneNotesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var user = await _authService.RegisterAsync(request.Email, request.Password);
        if (user == null) return BadRequest("Email is already in use.");

        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var data = await _authService.LoginAsync(request.Email, request.Password);
        if (data.Item1 == null) return Unauthorized("Invalid email or password.");

        return Ok(new {Token = data.Item1, Name = data.Item2, UserId = data.Item3});
    }
}

public record AuthRequest(string Email, string Password);