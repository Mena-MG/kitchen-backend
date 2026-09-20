using System.Security.Claims;
using KitchenApi.DTOs.Auth;
using KitchenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new Customer account.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(dto);

        if (!result.IsAuthenticated)
            return BadRequest(new { result.Message });

        return Ok(result);
    }

    /// <summary>
    /// Register a new Owner account (Owner-only).
    /// </summary>
    [Authorize(Roles = "Owner")]
    [HttpPost("register-owner")]
    public async Task<IActionResult> RegisterOwner([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterOwnerAsync(dto);

        if (!result.IsAuthenticated)
            return BadRequest(new { result.Message });

        return Ok(result);
    }

    /// <summary>
    /// Login with email and password → returns JWT + refresh token.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto);

        if (!result.IsAuthenticated)
            return Unauthorized(new { result.Message });

        return Ok(result);
    }

    /// <summary>
    /// Refresh an expired access token using a valid refresh token.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

        if (!result.IsAuthenticated)
            return Unauthorized(new { result.Message });

        return Ok(result);
    }

    /// <summary>
    /// Revoke a refresh token (logout).
    /// </summary>
    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenDto dto)
    {
        if (string.IsNullOrEmpty(dto.RefreshToken))
            return BadRequest(new { Message = "Refresh token is required." });

        var result = await _authService.RevokeTokenAsync(dto.RefreshToken);

        if (!result)
            return BadRequest(new { Message = "Invalid or already revoked token." });

        return NoContent();
    }

    /// <summary>
    /// Get the current authenticated user's profile and roles.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var firstName = User.FindFirstValue(ClaimTypes.GivenName);
        var lastName = User.FindFirstValue(ClaimTypes.Surname);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Ok(new
        {
            Id = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Roles = roles
        });
    }
}
