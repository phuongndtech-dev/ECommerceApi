using ECommerceApi.Application.DTOs.Common;
using ECommerceApi.Application.DTOs.Products;
using ECommerceApi.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductsController(
            IProductService productService,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator)
        {
            _productService = productService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all products with optional search and filtering
        /// </summary>
        /// <param name="search">Search term for product name</param>
        /// <param name="category">Filter by category</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="inStock">Filter by stock availability</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="sortBy">Sort field (default: Name)</param>
        /// <param name="sortDescending">Sort direction (default: false)</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts(
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? inStock = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "Name",
            [FromQuery] bool sortDescending = false)
        {
            var searchDto = new ProductSearchDto
            {
                Search = search,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                InStock = inStock,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _productService.GetAllAsync(searchDto);
            return HandleResult(result);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 404)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new product (Admin only)
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <returns>Created product</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            // Validate input
            var validationResult = await _createValidator.ValidateAsync(createProductDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<ProductDto>.FailureResult("Validation failed", errors);
                return BadRequest(response);
            }

            var userId = GetCurrentUserId();
            var result = await _productService.CreateAsync(createProductDto, userId);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result);
            }

            return HandleResult(result);
        }

        /// <summary>
        /// Update an existing product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update data</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            // Validate input
            var validationResult = await _updateValidator.ValidateAsync(updateProductDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<ProductDto>.FailureResult("Validation failed", errors);
                return BadRequest(response);
            }

            var result = await _productService.UpdateAsync(id, updateProductDto);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete a product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return HandleResult(result);
        }
    }
}