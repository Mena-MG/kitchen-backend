namespace KitchenApi.DTOs.Auth;

public class AuthResponseDto
{
    public bool IsAuthenticated { get; set; }
    public string? Message { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string> Roles { get; set; } = new();
    public string? Token { get; set; }
    public DateTime? TokenExpiresOn { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresOn { get; set; }
}
