using FluentAssertions;
using ECommerceApi.Application.DTOs.Products;
using ECommerceApi.Application.Validators;

namespace ECommerceApi.UnitTests.Application.Validators
{
    public class UpdateProductDtoValidatorTests
    {
        private readonly UpdateProductDtoValidator _validator;

        public UpdateProductDtoValidatorTests()
        {
            _validator = new UpdateProductDtoValidator();
        }

        [Fact]
        public void Validate_WithValidData_ShouldPass()
        {
            // Arrange
            var dto = new UpdateProductDto
            {
                Name = "Updated Product",
                Description = "Updated description",
                ImageUrl = "https://example.com/updated-image.jpg",
                Price = 149.99m,
                Stock = 75,
                Category = "Electronics",
                Sku = "UPD001",
                IsActive = true
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithIsActiveFalse_ShouldPass()
        {
            // Arrange
            var dto = CreateValidUpdateProductDto();
            dto.IsActive = false;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithIsActiveTrue_ShouldPass()
        {
            // Arrange
            var dto = CreateValidUpdateProductDto();
            dto.IsActive = true;

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        // Helper method
        private static UpdateProductDto CreateValidUpdateProductDto()
        {
            return new UpdateProductDto
            {
                Name = "Test Product",
                Description = "Test description",
                ImageUrl = "https://example.com/image.jpg",
                Price = 99.99m,
                Stock = 50,
                Category = "Electronics",
                Sku = "TEST001",
                IsActive = true
            };
        }
    }
}