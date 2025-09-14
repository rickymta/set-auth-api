using Microsoft.AspNetCore.Mvc;
using Set.Auth.Api.Controllers.Base;
using Set.Auth.Application.DTOs.Auth;
using Set.Auth.Application.Interfaces;

namespace Set.Auth.Api.Controllers;

/// <summary>
/// Controller for authentication operations including login, register, logout, and token management
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthController"/> class
/// </remarks>
/// <param name="authService">The authentication service</param>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : BaseController
{
    /// <summary>
    /// Authenticates a user and returns access and refresh tokens
    /// </summary>
    /// <param name="request">The login request containing user credentials</param>
    /// <returns>Authentication response with tokens and user information</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var userAgent = GetUserAgent();

            var result = await authService.LoginAsync(request, ipAddress, userAgent);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var userAgent = GetUserAgent();

            var result = await authService.RegisterAsync(request, ipAddress, userAgent);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var ipAddress = GetIpAddress();

            var result = await authService.RefreshTokenAsync(request, ipAddress);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        try
        {
            var ipAddress = GetIpAddress();

            await authService.LogoutAsync(request, ipAddress);
            return Ok(SuccessMessage("Logged out successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    [HttpPost("logout-all")]
    public async Task<ActionResult> LogoutAll([FromBody] LogoutAllRequestDto request)
    {
        try
        {
            var ipAddress = GetIpAddress();

            await authService.LogoutAllDevicesAsync(request, ipAddress);
            return Ok(SuccessMessage("Logged out from all devices successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    [HttpGet("validate-token")]
    public async Task<ActionResult> ValidateToken([FromQuery] string token)
    {
        try
        {
            var isValid = await authService.ValidateTokenAsync(token);
            return Ok(SuccessData(new { isValid }));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    private string? GetIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value))
        {
            return value;
        }

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    private string? GetUserAgent()
    {
        return Request.Headers.UserAgent;
    }
}
