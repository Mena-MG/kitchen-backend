using System.ComponentModel.DataAnnotations;

namespace KitchenApi.Models;

public class PickupLocation
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string NameAr { get; set; } = string.Empty;

    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(250)]
    public string AddressAr { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Hours { get; set; } = string.Empty;

    [MaxLength(150)]
    public string HoursAr { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(150)]
    public string StockStatus { get; set; } = string.Empty;

    [MaxLength(150)]
    public string StockStatusAr { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
}
