using System.ComponentModel.DataAnnotations;

namespace KitchenApi.DTOs.Locations;

public class CreateLocationDto
{
    public string? Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string NameAr { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
    public string AddressAr { get; set; } = string.Empty;
    public string Hours { get; set; } = string.Empty;
    public string HoursAr { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string StockStatus { get; set; } = string.Empty;
    public string StockStatusAr { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
}

public class LocationResponseDto : CreateLocationDto
{
    public new string Id { get; set; } = string.Empty;
}
