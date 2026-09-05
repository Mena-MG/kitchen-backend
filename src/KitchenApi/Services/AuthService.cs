using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KitchenApi.Data;
using KitchenApi.DTOs.Auth;
using KitchenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KitchenApi.Services;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 30;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSettings _jwt;
    private readonly AppDbContext _db;

    public AuthService(
        UserManager<AppUser> userManager,
        IOptions<JwtSettings> jwt,
        AppDbContext db)
    {
        _userManager = userManager;
        _jwt = jwt.Value;
        _db = db;
    }

    // ── Register a new Customer ──
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check if email is already registered
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
        {
            return new AuthResponseDto { Message = "Email is already registered." };
        }

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponseDto { Message = $"Registration failed: {errors}" };
        }

        // Assign Customer role by default
        await _userManager.AddToRoleAsync(user, "Customer");

        // Generate tokens
        var jwtToken = await GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        // Save refresh token
        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = new List<string> { "Customer" },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            TokenExpiresOn = jwtToken.ValidTo,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresOn = refreshToken.ExpiresOn
        };
    }

    // ── Register a new Owner (only callable by existing Owner) ──
    public async Task<AuthResponseDto> RegisterOwnerAsync(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
        {
            return new AuthResponseDto { Message = "Email is already registered." };
        }

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponseDto { Message = $"Registration failed: {errors}" };
        }

        await _userManager.AddToRoleAsync(user, "Owner");

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Message = "Owner account created successfully.",
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = new List<string> { "Owner" }
        };
    }

    // ── Login ──
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return new AuthResponseDto { Message = "Invalid email or password." };
        }

        var jwtToken = await GenerateJwtToken(user);
        var roles = await _userManager.GetRolesAsync(user);

        // Check for an existing active refresh token, or generate a new one
        var activeRefreshToken = await _db.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedOn == null && rt.ExpiresOn > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        RefreshToken refreshToken;
        if (activeRefreshToken is not null)
        {
            refreshToken = activeRefreshToken;
        }
        else
        {
            refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList(),
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            TokenExpiresOn = jwtToken.ValidTo,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresOn = refreshToken.ExpiresOn
        };
    }

    // ── Refresh an expired access token ──
    public async Task<AuthResponseDto> RefreshTokenAsync(string token)
    {
        var storedToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (storedToken is null)
        {
            return new AuthResponseDto { Message = "Invalid refresh token." };
        }

        if (!storedToken.IsActive)
        {
            return new AuthResponseDto { Message = "Refresh token is expired or revoked." };
        }

        // Revoke old refresh token and create a new one (rotation)
        storedToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();
        storedToken.User.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        // Generate new JWT
        var jwtToken = await GenerateJwtToken(storedToken.User);
        var roles = await _userManager.GetRolesAsync(storedToken.User);

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Email = storedToken.User.Email,
            FirstName = storedToken.User.FirstName,
            LastName = storedToken.User.LastName,
            Roles = roles.ToList(),
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            TokenExpiresOn = jwtToken.ValidTo,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresOn = newRefreshToken.ExpiresOn
        };
    }

    // ── Revoke a refresh token (logout) ──
    public async Task<bool> RevokeTokenAsync(string token)
    {
        var storedToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (storedToken is null || !storedToken.IsActive)
            return false;

        storedToken.RevokedOn = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Private: Generate JWT ──
    private async Task<JwtSecurityToken> GenerateJwtToken(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Email!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return token;
    }

    // ── Private: Generate Refresh Token ──
    private RefreshToken GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            ExpiresOn = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays),
            CreatedOn = DateTime.UtcNow
        };
    }
}
