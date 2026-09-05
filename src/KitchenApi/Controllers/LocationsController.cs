using KitchenApi.Data;
using KitchenApi.DTOs.Locations;
using KitchenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LocationsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// List all showrooms and stone workshops
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PickupLocation>>> GetLocations()
    {
        var locations = await _context.Locations.AsNoTracking().ToListAsync();
        return Ok(locations);
    }

    /// <summary>
    /// Create new showroom or workshop location (Owner only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<PickupLocation>> CreateLocation([FromBody] CreateLocationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var id = !string.IsNullOrWhiteSpace(dto.Id) ? dto.Id : $"loc-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var location = new PickupLocation
        {
            Id = id,
            Name = dto.Name,
            NameAr = dto.NameAr,
            Address = dto.Address,
            AddressAr = dto.AddressAr,
            Hours = dto.Hours,
            HoursAr = dto.HoursAr,
            Phone = dto.Phone,
            StockStatus = dto.StockStatus,
            StockStatusAr = dto.StockStatusAr,
            IsDefault = dto.IsDefault
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLocations), new { id = location.Id }, location);
    }

    /// <summary>
    /// Update showroom location (Owner only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<PickupLocation>> UpdateLocation(string id, [FromBody] CreateLocationDto dto)
    {
        var loc = await _context.Locations.FindAsync(id);
        if (loc == null) return NotFound(new { message = $"Location '{id}' not found." });

        loc.Name = dto.Name;
        loc.NameAr = dto.NameAr;
        loc.Address = dto.Address;
        loc.AddressAr = dto.AddressAr;
        loc.Hours = dto.Hours;
        loc.HoursAr = dto.HoursAr;
        loc.Phone = dto.Phone;
        loc.StockStatus = dto.StockStatus;
        loc.StockStatusAr = dto.StockStatusAr;
        loc.IsDefault = dto.IsDefault;

        await _context.SaveChangesAsync();
        return Ok(loc);
    }

    /// <summary>
    /// Delete showroom location (Owner only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteLocation(string id)
    {
        var loc = await _context.Locations.FindAsync(id);
        if (loc == null) return NotFound(new { message = $"Location '{id}' not found." });

        _context.Locations.Remove(loc);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
