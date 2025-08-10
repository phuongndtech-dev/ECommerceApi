using AutoMapper;
using ECommerceApi.Application.DTOs.Products;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using ECommerceApi.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ECommerceApi.UnitTests.Application.Services
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _mapperMock = new Mock<IMapper>();
            _productService = new ProductService(_context, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var createDto = new CreateProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                ImageUrl = "https://example.com/image.jpg",
                Price = 99.99m,
                Stock = 50,
                Category = "Electronics",
                Sku = "TEST001"
            };

            // Mock what mapper should return
            var mappedProduct = new Product
            {
                Name = createDto.Name,
                Description = createDto.Description,
                ImageUrl = createDto.ImageUrl,
                Price = createDto.Price,
                Stock = createDto.Stock,
                Category = createDto.Category,
                Sku = createDto.Sku,
                IsActive = true
            };

            var resultDto = new ProductDto
            {
                Id = 1,
                Name = createDto.Name,
                Description = createDto.Description,
                ImageUrl = createDto.ImageUrl,
                Price = createDto.Price,
                Stock = createDto.Stock,
                Category = createDto.Category,
                Sku = createDto.Sku,
                IsActive = true,
                IsInStock = true,
                CreatedBy = "Test User"
            };

            // Setup mapper expectations
            _mapperMock.Setup(m => m.Map<Product>(createDto))
                       .Returns(mappedProduct);

            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                       .Returns(resultDto);

            // Act
            var result = await _productService.CreateAsync(createDto, user.Id);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);
            result.Data.Price.Should().Be(createDto.Price);
            result.Data.IsInStock.Should().BeTrue();
            result.Message.Should().Be("Product created successfully");

            // Verify mapper was called correctly
            _mapperMock.Verify(m => m.Map<Product>(createDto), Times.Once);
            _mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateSku_ShouldReturnFailure()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            // Create existing product in database
            var existingProduct = new Product
            {
                Name = "Existing Product",
                Sku = "DUPLICATE001",
                Price = 100m,
                Stock = 10,
                Category = "Electronics",
                Description = "Existing",
                ImageUrl = "https://example.com/existing.jpg",
                IsActive = true,
                CreatedByUserId = user.Id
            };
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "New Description",
                ImageUrl = "https://example.com/new.jpg",
                Price = 200m,
                Stock = 20,
                Category = "Electronics",
                Sku = "DUPLICATE001" // Same SKU!
            };

            // Act
            var result = await _productService.CreateAsync(createDto, user.Id);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Product with this SKU already exists");

            // Verify mapper was NOT called (early return)
            _mapperMock.Verify(m => m.Map<Product>(It.IsAny<CreateProductDto>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var product = await CreateTestProductAsync(user.Id);

            var expectedDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                Sku = product.Sku,
                IsActive = product.IsActive,
                IsInStock = true,
                CreatedBy = "Test User"
            };

            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                       .Returns(expectedDto);

            // Act
            var result = await _productService.GetByIdAsync(product.Id);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(product.Id);
            result.Data.Name.Should().Be(product.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var result = await _productService.GetByIdAsync(nonExistentId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Product not found");

            // Mapper should not be called
            _mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var product = await CreateTestProductAsync(user.Id);

            // Act
            var result = await _productService.DeleteAsync(product.Id);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Product deleted successfully");

            // Verify product is actually deleted from database
            var deletedProduct = await _context.Products.FindAsync(product.Id);
            deletedProduct.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var result = await _productService.DeleteAsync(nonExistentId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Product not found");
        }

        [Fact]
        public async Task GetAllAsync_WithNoFilters_ShouldReturnAllProducts()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await CreateTestProductAsync(user.Id, "Product 1");
            await CreateTestProductAsync(user.Id, "Product 2");
            await CreateTestProductAsync(user.Id, "Product 3");

            var searchDto = new ProductSearchDto { Page = 1, PageSize = 10 };

            var expectedDtos = new List<ProductDto>
            {
                new() { Id = 1, Name = "Product 1", IsInStock = true },
                new() { Id = 2, Name = "Product 2", IsInStock = true },
                new() { Id = 3, Name = "Product 3", IsInStock = true }
            };

            _mapperMock.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
                       .Returns(expectedDtos);

            // Act
            var result = await _productService.GetAllAsync(searchDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Items.Should().HaveCount(3);
            result.Data.TotalCount.Should().Be(3);
            result.Data.Page.Should().Be(1);
            result.Data.PageSize.Should().Be(10);
        }

        // Helper methods
        private async Task<User> CreateTestUserAsync()
        {
            var user = new User
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                PasswordHash = "hashedpassword",
                FirstName = "Test",
                LastName = "User",
                Role = UserRole.Admin,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<Product> CreateTestProductAsync(int createdByUserId, string name = "Test Product")
        {
            var product = new Product
            {
                Name = name,
                Description = "Test Description",
                ImageUrl = "https://example.com/image.jpg",
                Price = 99.99m,
                Stock = 50,
                Category = "Electronics",
                Sku = $"SKU{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                IsActive = true,
                CreatedByUserId = createdByUserId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}