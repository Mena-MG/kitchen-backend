using System.Text.Json;
using KitchenApi.Data;
using KitchenApi.DTOs.Products;
using KitchenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ProductResponseDto>> GetProductsAsync(ProductQueryParameters query)
    {
        var dbQuery = _context.Products.AsNoTracking().AsQueryable();

        // Search filter (name, description, material, color, origin)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            dbQuery = dbQuery.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.NameAr.ToLower().Contains(search) ||
                p.Description.ToLower().Contains(search) ||
                p.DescriptionAr.ToLower().Contains(search) ||
                p.Material.ToLower().Contains(search) ||
                p.MaterialAr.ToLower().Contains(search) ||
                p.CategoryName.ToLower().Contains(search) ||
                p.CategoryNameAr.ToLower().Contains(search)
            );
        }

        // Category filter (by ID, slug, or Name)
        if (!string.IsNullOrWhiteSpace(query.Category) && query.Category.ToLower() != "all")
        {
            var cat = query.Category.Trim().ToLower();
            dbQuery = dbQuery.Where(p =>
                (p.CategoryId != null && p.CategoryId.ToLower() == cat) ||
                p.CategoryName.ToLower().Contains(cat) ||
                p.CategoryNameAr.ToLower().Contains(cat)
            );
        }

        // SurfaceType filter (countertop, cabinet, backsplash)
        if (!string.IsNullOrWhiteSpace(query.SurfaceType) && query.SurfaceType.ToLower() != "all")
        {
            var surface = query.SurfaceType.Trim().ToLower();
            dbQuery = dbQuery.Where(p => p.SurfaceType.ToLower() == surface);
        }

        // Material filter
        if (!string.IsNullOrWhiteSpace(query.Material))
        {
            var mat = query.Material.Trim().ToLower();
            dbQuery = dbQuery.Where(p => p.Material.ToLower().Contains(mat) || p.MaterialAr.ToLower().Contains(mat));
        }

        // Featured / BestSeller / InStock flags
        if (query.Featured.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.IsFeatured == query.Featured.Value);
        }

        if (query.BestSeller.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.IsBestSeller == query.BestSeller.Value);
        }

        if (query.InStock.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.InStock == query.InStock.Value);
        }

        // Sorting
        dbQuery = query.SortBy?.ToLower() switch
        {
            "price_asc" => dbQuery.OrderBy(p => p.Price),
            "price_desc" => dbQuery.OrderByDescending(p => p.Price),
            "rating" => dbQuery.OrderByDescending(p => p.Rating),
            "popular" => dbQuery.OrderByDescending(p => p.ReviewsCount),
            _ => dbQuery.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await dbQuery.CountAsync();
        var pageNumber = Math.Max(1, query.PageNumber);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await dbQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ProductResponseDto>
        {
            Items = items.Select(MapToResponseDto).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(string id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return product == null ? null : MapToResponseDto(product);
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        var id = !string.IsNullOrWhiteSpace(dto.Id) ? dto.Id : $"prod-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var product = new Product
        {
            Id = id,
            Name = dto.Name,
            NameAr = dto.NameAr,
            Price = dto.Price,
            OriginalPrice = dto.OriginalPrice > 0 ? dto.OriginalPrice : dto.Price,
            UnitType = dto.UnitType,
            SurfaceType = dto.SurfaceType ?? "none",
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            CategoryNameAr = dto.CategoryNameAr,
            Material = dto.Material,
            MaterialAr = dto.MaterialAr,
            Finish = dto.Finish,
            FinishAr = dto.FinishAr,
            Color = dto.Color,
            ColorAr = dto.ColorAr,
            ColorHex = dto.ColorHex,
            Thickness = dto.Thickness,
            OriginCountry = dto.OriginCountry,
            OriginCountryAr = dto.OriginCountryAr,
            ImageUrl = dto.ImageUrl,
            VideoUrl = dto.VideoUrl,
            Badge = dto.Badge,
            BadgeAr = dto.BadgeAr,
            IsFeatured = dto.IsFeatured,
            IsBestSeller = dto.IsBestSeller,
            InStock = dto.InStock,
            ShortDescription = dto.ShortDescription,
            ShortDescriptionAr = dto.ShortDescriptionAr,
            Description = dto.Description,
            DescriptionAr = dto.DescriptionAr,
            FeaturesJson = JsonSerializer.Serialize(dto.Features ?? new List<string>()),
            FeaturesArJson = JsonSerializer.Serialize(dto.FeaturesAr ?? new List<string>()),
            GalleryJson = JsonSerializer.Serialize(dto.Gallery ?? new List<string>()),
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Product created: {Id} - {Name}", product.Id, product.Name);

        return MapToResponseDto(product);
    }

    public async Task<ProductResponseDto?> UpdateProductAsync(string id, UpdateProductDto dto)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.NameAr = dto.NameAr;
        product.Price = dto.Price;
        product.OriginalPrice = dto.OriginalPrice;
        product.UnitType = dto.UnitType;
        product.SurfaceType = dto.SurfaceType ?? product.SurfaceType;
        product.CategoryId = dto.CategoryId;
        product.CategoryName = dto.CategoryName;
        product.CategoryNameAr = dto.CategoryNameAr;
        product.Material = dto.Material;
        product.MaterialAr = dto.MaterialAr;
        product.Finish = dto.Finish;
        product.FinishAr = dto.FinishAr;
        product.Color = dto.Color;
        product.ColorAr = dto.ColorAr;
        product.ColorHex = dto.ColorHex;
        product.Thickness = dto.Thickness;
        product.OriginCountry = dto.OriginCountry;
        product.OriginCountryAr = dto.OriginCountryAr;
        product.ImageUrl = dto.ImageUrl;
        product.VideoUrl = dto.VideoUrl;
        product.Badge = dto.Badge;
        product.BadgeAr = dto.BadgeAr;
        product.IsFeatured = dto.IsFeatured;
        product.IsBestSeller = dto.IsBestSeller;
        product.InStock = dto.InStock;
        product.ShortDescription = dto.ShortDescription;
        product.ShortDescriptionAr = dto.ShortDescriptionAr;
        product.Description = dto.Description;
        product.DescriptionAr = dto.DescriptionAr;
        if (dto.Features != null) product.FeaturesJson = JsonSerializer.Serialize(dto.Features);
        if (dto.FeaturesAr != null) product.FeaturesArJson = JsonSerializer.Serialize(dto.FeaturesAr);
        if (dto.Gallery != null) product.GalleryJson = JsonSerializer.Serialize(dto.Gallery);
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Product updated: {Id}", product.Id);

        return MapToResponseDto(product);
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Product deleted: {Id}", id);
        return true;
    }

    public async Task<bool> SeedDefaultProductsAsync()
    {
        await DbInitializer.SeedEcommerceDataAsync(_context, _logger);
        return true;
    }

    private static ProductResponseDto MapToResponseDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        NameAr = p.NameAr,
        Price = p.Price,
        OriginalPrice = p.OriginalPrice,
        UnitType = p.UnitType,
        SurfaceType = p.SurfaceType,
        CategoryId = p.CategoryId,
        CategoryName = p.CategoryName,
        CategoryNameAr = p.CategoryNameAr,
        Material = p.Material,
        MaterialAr = p.MaterialAr,
        Finish = p.Finish,
        FinishAr = p.FinishAr,
        Color = p.Color,
        ColorAr = p.ColorAr,
        ColorHex = p.ColorHex,
        Thickness = p.Thickness,
        OriginCountry = p.OriginCountry,
        OriginCountryAr = p.OriginCountryAr,
        ImageUrl = p.ImageUrl,
        VideoUrl = p.VideoUrl,
        Badge = p.Badge,
        BadgeAr = p.BadgeAr,
        IsFeatured = p.IsFeatured,
        IsBestSeller = p.IsBestSeller,
        InStock = p.InStock,
        Rating = p.Rating,
        ReviewsCount = p.ReviewsCount,
        ShortDescription = p.ShortDescription,
        ShortDescriptionAr = p.ShortDescriptionAr,
        Description = p.Description,
        DescriptionAr = p.DescriptionAr,
        Features = SafeDeserializeList(p.FeaturesJson),
        FeaturesAr = SafeDeserializeList(p.FeaturesArJson),
        Gallery = SafeDeserializeList(p.GalleryJson),
        CreatedAt = p.CreatedAt
    };

    private static List<string> SafeDeserializeList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
