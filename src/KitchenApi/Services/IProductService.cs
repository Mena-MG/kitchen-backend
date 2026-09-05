using KitchenApi.DTOs.Products;

namespace KitchenApi.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetProductsAsync(ProductQueryParameters query);
    Task<ProductResponseDto?> GetProductByIdAsync(string id);
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductResponseDto?> UpdateProductAsync(string id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(string id);
    Task<bool> SeedDefaultProductsAsync();
}
