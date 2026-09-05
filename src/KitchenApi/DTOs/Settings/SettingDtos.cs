using System.ComponentModel.DataAnnotations;

namespace KitchenApi.DTOs.Settings;

public class UpdateSettingDto
{
    [Required]
    public string Value { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class SettingResponseDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
