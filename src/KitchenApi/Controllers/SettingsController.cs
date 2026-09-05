using KitchenApi.Data;
using KitchenApi.DTOs.Settings;
using KitchenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SettingsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all store settings
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Dictionary<string, string>>> GetSettings()
    {
        var settings = await _context.Settings
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Key, s => s.Value);

        return Ok(settings);
    }

    /// <summary>
    /// Update or create store setting (Owner only)
    /// </summary>
    [HttpPut("{key}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<SettingResponseDto>> UpdateSetting(string key, [FromBody] UpdateSettingDto dto)
    {
        var setting = await _context.Settings.FindAsync(key);
        if (setting == null)
        {
            setting = new StoreSetting
            {
                Key = key,
                Value = dto.Value,
                Description = dto.Description,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Settings.Add(setting);
        }
        else
        {
            setting.Value = dto.Value;
            if (!string.IsNullOrEmpty(dto.Description)) setting.Description = dto.Description;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(new SettingResponseDto
        {
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description,
            UpdatedAt = setting.UpdatedAt
        });
    }
}
