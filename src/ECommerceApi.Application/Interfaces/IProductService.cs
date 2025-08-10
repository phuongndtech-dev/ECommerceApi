using ECommerceApi.Application.DTOs.Products;
using ECommerceApi.Application.DTOs.Common;

namespace ECommerceApi.Application.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto createDto, int createdByUserId);
        Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto updateDto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<ProductDto>> GetByIdAsync(int id);
        Task<ApiResponse<PagedResult<ProductDto>>> GetAllAsync(ProductSearchDto searchDto);
    }
}