using KitchenApi.Data;
using KitchenApi.DTOs.Promos;
using KitchenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PromosController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Validate promo code against cart subtotal
    /// </summary>
    [HttpPost("validate")]
    [AllowAnonymous]
    public async Task<ActionResult<PromoValidationResultDto>> ValidatePromo([FromBody] ValidatePromoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            return BadRequest(new { message = "Promo code is required." });
        }

        var promo = await _context.PromoCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Code.ToUpper() == dto.Code.Trim().ToUpper());

        if (promo == null || !promo.IsActive)
        {
            return Ok(new PromoValidationResultDto
            {
                IsValid = false,
                Code = dto.Code,
                Message = "Invalid or expired promo code."
            });
        }

        if (dto.Subtotal < promo.MinOrder)
        {
            return Ok(new PromoValidationResultDto
            {
                IsValid = false,
                Code = promo.Code,
                Message = $"Promo code requires a minimum order of {promo.MinOrder:N0} EGP."
            });
        }

        decimal discountAmount = 0;
        if (promo.DiscountPercent > 0)
        {
            discountAmount = Math.Round(dto.Subtotal * promo.DiscountPercent / 100m, 2);
        }
        else if (promo.FixedDiscount > 0)
        {
            discountAmount = Math.Min(promo.FixedDiscount, dto.Subtotal);
        }

        return Ok(new PromoValidationResultDto
        {
            IsValid = true,
            Code = promo.Code,
            Message = "Promo code applied successfully.",
            DiscountPercent = promo.DiscountPercent,
            FixedDiscount = promo.FixedDiscount,
            DiscountAmount = discountAmount,
            NewTotal = Math.Max(0, dto.Subtotal - discountAmount),
            Label = promo.Label,
            LabelAr = promo.LabelAr
        });
    }

    /// <summary>
    /// List all promo codes (Owner only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<List<PromoCode>>> GetAllPromos()
    {
        var promos = await _context.PromoCodes.AsNoTracking().ToListAsync();
        return Ok(promos);
    }

    /// <summary>
    /// Create promo code (Owner only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<PromoCode>> CreatePromo([FromBody] CreatePromoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existing = await _context.PromoCodes.FindAsync(dto.Code.Trim().ToUpper());
        if (existing != null)
        {
            return BadRequest(new { message = $"Promo code '{dto.Code}' already exists." });
        }

        var promo = new PromoCode
        {
            Code = dto.Code.Trim().ToUpper(),
            DiscountPercent = dto.DiscountPercent,
            FixedDiscount = dto.FixedDiscount,
            Label = dto.Label,
            LabelAr = dto.LabelAr,
            MinOrder = dto.MinOrder,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.PromoCodes.Add(promo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllPromos), new { code = promo.Code }, promo);
    }
}
