using FluentAssertions;
using ECommerceApi.Application.DTOs.Products;
using ECommerceApi.Application.Validators;

namespace ECommerceApi.UnitTests.Application.Validators
{
    public class CreateProductDtoValidatorTests
    {
        private readonly CreateProductDtoValidator _validator;

        public CreateProductDtoValidatorTests()
        {
            _validator = new CreateProductDtoValidator();
        }

        [Fact]
        public void Validate_WithValidData_ShouldPass()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "iPhone 15 Pro",
                Description = "Latest iPhone with advanced features",
                ImageUrl = "https://example.com/image.jpg",
                Price = 999.99m,
                Stock = 100,
                Category = "Electronics",
                Sku = "IPH15PRO001"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Validate_WithEmptyName_ShouldFail(string name)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Name = name;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Name));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void Validate_WithTooLongName_ShouldFail()
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Name = new string('A', 201); // Exceeds 200 character limit

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Name));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("200 characters"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Validate_WithEmptyDescription_ShouldFail(string description)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Description = description;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Description));
        }

        [Fact]
        public void Validate_WithTooLongDescription_ShouldFail()
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Description = new string('A', 2001); // Exceeds 2000 character limit

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Description));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("2000 characters"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("not-a-url")]
        [InlineData("http://")]
        [InlineData("ftp://example.com/image.jpg")]
        public void Validate_WithInvalidImageUrl_ShouldFail(string imageUrl)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.ImageUrl = imageUrl;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.ImageUrl));
        }

        [Theory]
        [InlineData("https://example.com/image.jpg")]
        [InlineData("http://example.com/image.png")]
        [InlineData("https://cdn.example.com/products/image.gif")]
        public void Validate_WithValidImageUrl_ShouldPass(string imageUrl)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.ImageUrl = imageUrl;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99.99)]
        public void Validate_WithInvalidPrice_ShouldFail(decimal price)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Price = price;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Price));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("greater than 0"));
        }

        [Fact]
        public void Validate_WithTooHighPrice_ShouldFail()
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Price = 1000000.00m; // Exceeds 999,999.99 limit

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Price));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("999,999.99"));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Validate_WithNegativeStock_ShouldFail(int stock)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Stock = stock;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Stock));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("cannot be negative"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999999)]
        public void Validate_WithValidStock_ShouldPass(int stock)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Stock = stock;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Validate_WithEmptyCategory_ShouldFail(string category)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Category = category;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Category));
        }

        [Fact]
        public void Validate_WithTooLongCategory_ShouldFail()
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Category = new string('A', 101); // Exceeds 100 character limit

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Category));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("100 characters"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Validate_WithEmptySku_ShouldFail(string sku)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Sku = sku;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Sku));
        }

        [Theory]
        [InlineData("sku123")]        // Lowercase
        [InlineData("SKU-123")]       // Contains hyphen
        [InlineData("SKU 123")]       // Contains space
        [InlineData("SKU@123")]       // Contains special char
        public void Validate_WithInvalidSkuFormat_ShouldFail(string sku)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Sku = sku;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Sku));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("uppercase letters and numbers"));
        }

        [Theory]
        [InlineData("SKU123")]
        [InlineData("PRODUCT001")]
        [InlineData("IPH15PRO001")]
        [InlineData("ABC123XYZ")]
        public void Validate_WithValidSkuFormat_ShouldPass(string sku)
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Sku = sku;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithTooLongSku_ShouldFail()
        {
            // Arrange
            var dto = CreateValidProductDto();
            dto.Sku = new string('A', 51); // Exceeds 50 character limit

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductDto.Sku));
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("50 characters"));
        }

        [Fact]
        public void Validate_WithMultipleErrors_ShouldReturnAllErrors()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "", // Invalid: empty
                Description = "", // Invalid: empty
                ImageUrl = "not-a-url", // Invalid: not URL
                Price = -1, // Invalid: negative
                Stock = -1, // Invalid: negative
                Category = "", // Invalid: empty
                Sku = "invalid-sku" // Invalid: lowercase with hyphen
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThan(5);

            // Should have errors for all invalid fields
            var errorProperties = result.Errors.Select(e => e.PropertyName).ToList();
            errorProperties.Should().Contain(nameof(CreateProductDto.Name));
            errorProperties.Should().Contain(nameof(CreateProductDto.Description));
            errorProperties.Should().Contain(nameof(CreateProductDto.ImageUrl));
            errorProperties.Should().Contain(nameof(CreateProductDto.Price));
            errorProperties.Should().Contain(nameof(CreateProductDto.Stock));
            errorProperties.Should().Contain(nameof(CreateProductDto.Category));
            errorProperties.Should().Contain(nameof(CreateProductDto.Sku));
        }

        // Helper method to create a valid product DTO for testing
        private static CreateProductDto CreateValidProductDto()
        {
            return new CreateProductDto
            {
                Name = "Test Product",
                Description = "This is a test product description",
                ImageUrl = "https://example.com/image.jpg",
                Price = 99.99m,
                Stock = 50,
                Category = "Electronics",
                Sku = "TEST001"
            };
        }
    }
}