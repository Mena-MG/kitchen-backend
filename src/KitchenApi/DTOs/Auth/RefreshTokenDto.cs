using System.ComponentModel.DataAnnotations;

namespace KitchenApi.DTOs.Auth;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
