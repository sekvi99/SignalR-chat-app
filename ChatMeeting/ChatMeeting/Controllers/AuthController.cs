using ChatMeeting.Core.Domain.Dtos;
using ChatMeeting.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatMeeting.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    [HttpPut("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto user)
    {
        try
        {
            await _authService.RegisterUserAsync(user);
            return Ok(new { message = "User registered successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginModel)
    {
        try
        {
            var authData = await _authService.GetTokenAsync(loginModel);
            return Ok(authData);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
