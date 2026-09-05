using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitchenApi.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;

    public DateTime ExpiresOn { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedOn { get; set; }

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

    [NotMapped]
    public bool IsRevoked => RevokedOn is not null;

    [NotMapped]
    public bool IsActive => !IsExpired && !IsRevoked;
}
