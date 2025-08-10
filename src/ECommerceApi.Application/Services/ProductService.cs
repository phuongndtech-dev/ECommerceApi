using AutoMapper;
using ECommerceApi.Application.DTOs.Common;
using ECommerceApi.Application.DTOs.Products;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto createDto, int createdByUserId)
        {
            try
            {
                // Check if SKU already exists
                var existingSku = await _context.Products
                    .FirstOrDefaultAsync(p => p.Sku == createDto.Sku);

                if (existingSku != null)
                {
                    return ApiResponse<ProductDto>.FailureResult("Product with this SKU already exists");
                }

                var product = _mapper.Map<Product>(createDto);
                product.CreatedByUserId = createdByUserId;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Load the product with navigation properties
                await _context.Entry(product)
                    .Reference(p => p.CreatedBy)
                    .LoadAsync();

                var productDto = _mapper.Map<ProductDto>(product);
                return ApiResponse<ProductDto>.SuccessResult(productDto, "Product created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.FailureResult("An error occurred while creating the product", ex.Message);
            }
        }

        public async Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto updateDto)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.CreatedBy)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return ApiResponse<ProductDto>.FailureResult("Product not found");
                }

                // Check if SKU is being changed and if it conflicts
                if (product.Sku != updateDto.Sku)
                {
                    var existingSku = await _context.Products
                        .FirstOrDefaultAsync(p => p.Sku == updateDto.Sku && p.Id != id);

                    if (existingSku != null)
                    {
                        return ApiResponse<ProductDto>.FailureResult("Product with this SKU already exists");
                    }
                }

                _mapper.Map(updateDto, product);
                await _context.SaveChangesAsync();

                var productDto = _mapper.Map<ProductDto>(product);
                return ApiResponse<ProductDto>.SuccessResult(productDto, "Product updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.FailureResult("An error occurred while updating the product", ex.Message);
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return ApiResponse<string>.FailureResult("Product not found");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResult("Product deleted", "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailureResult("An error occurred while deleting the product", ex.Message);
            }
        }

        public async Task<ApiResponse<ProductDto>> GetByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.CreatedBy)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return ApiResponse<ProductDto>.FailureResult("Product not found");
                }

                var productDto = _mapper.Map<ProductDto>(product);
                return ApiResponse<ProductDto>.SuccessResult(productDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.FailureResult("An error occurred while fetching the product", ex.Message);
            }
        }

        public async Task<ApiResponse<PagedResult<ProductDto>>> GetAllAsync(ProductSearchDto searchDto)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.CreatedBy)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchDto.Search))
                {
                    query = query.Where(p => p.Name.Contains(searchDto.Search) ||
                                           p.Description.Contains(searchDto.Search));
                }

                if (!string.IsNullOrEmpty(searchDto.Category))
                {
                    query = query.Where(p => p.Category == searchDto.Category);
                }

                if (searchDto.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= searchDto.MinPrice.Value);
                }

                if (searchDto.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= searchDto.MaxPrice.Value);
                }

                if (searchDto.InStock.HasValue)
                {
                    if (searchDto.InStock.Value)
                    {
                        query = query.Where(p => p.Stock > 0 && p.IsActive);
                    }
                    else
                    {
                        query = query.Where(p => p.Stock == 0 || !p.IsActive);
                    }
                }

                // Apply sorting
                query = searchDto.SortBy.ToLower() switch
                {
                    "price" => searchDto.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "createdat" => searchDto.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                    "stock" => searchDto.SortDescending ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock),
                    _ => searchDto.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
                };

                var totalCount = await query.CountAsync();

                var products = await query
                    .Skip((searchDto.Page - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToListAsync();

                var productDtos = _mapper.Map<List<ProductDto>>(products);

                var pagedResult = new PagedResult<ProductDto>
                {
                    Items = productDtos,
                    TotalCount = totalCount,
                    Page = searchDto.Page,
                    PageSize = searchDto.PageSize
                };

                return ApiResponse<PagedResult<ProductDto>>.SuccessResult(pagedResult);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<ProductDto>>.FailureResult("An error occurred while fetching products", ex.Message);
            }
        }
    }
}