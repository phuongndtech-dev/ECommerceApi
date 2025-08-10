using ECommerceApi.Domain.Entities;
using FluentAssertions;

namespace ECommerceApi.UnitTests.Domain
{
    public class ProductTests
    {
        [Fact]
        public void IsInStock_ShouldReturnTrueWhenActiveAndHasStock()
        {
            // Arrange
            var product = new Product
            {
                Stock = 10,
                IsActive = true
            };

            // Act
            var result = product.IsInStock();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInStock_ShouldReturnFalseWhenNoStock()
        {
            // Arrange
            var product = new Product
            {
                Stock = 0,
                IsActive = true
            };

            // Act
            var result = product.IsInStock();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsInStock_ShouldReturnFalseWhenNotActive()
        {
            // Arrange
            var product = new Product
            {
                Stock = 10,
                IsActive = false
            };

            // Act
            var result = product.IsInStock();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CanPurchase_ShouldReturnTrueWhenSufficientStock()
        {
            // Arrange
            var product = new Product
            {
                Stock = 10,
                IsActive = true
            };

            // Act
            var result = product.CanPurchase(5);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CanPurchase_ShouldReturnFalseWhenInsufficientStock()
        {
            // Arrange
            var product = new Product
            {
                Stock = 3,
                IsActive = true
            };

            // Act
            var result = product.CanPurchase(5);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void UpdateStock_ShouldUpdateStockAndTimestamp()
        {
            // Arrange
            var product = new Product { Stock = 10 };
            var beforeUpdate = DateTime.UtcNow;

            // Act
            product.UpdateStock(15);

            // Assert
            product.Stock.Should().Be(15);
            product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void UpdateStock_ShouldThrowExceptionForNegativeStock()
        {
            // Arrange
            var product = new Product { Stock = 10 };

            // Act & Assert
            var action = () => product.UpdateStock(-5);
            action.Should().Throw<ArgumentException>()
                  .WithMessage("Stock cannot be negative");
        }

        [Fact]
        public void DeductStock_ShouldReduceStock()
        {
            // Arrange
            var product = new Product
            {
                Stock = 10,
                IsActive = true
            };

            // Act
            product.DeductStock(3);

            // Assert
            product.Stock.Should().Be(7);
        }

        [Fact]
        public void DeductStock_ShouldThrowExceptionWhenInsufficientStock()
        {
            // Arrange
            var product = new Product
            {
                Stock = 2,
                IsActive = true
            };

            // Act & Assert
            var action = () => product.DeductStock(5);
            action.Should().Throw<InvalidOperationException>()
                  .WithMessage("Cannot deduct 5 items. Available stock: 2");
        }

        [Fact]
        public void GetTotalPrice_ShouldCalculateCorrectTotal()
        {
            // Arrange
            var product = new Product { Price = 10.50m };

            // Act
            var total = product.GetTotalPrice(3);

            // Assert
            total.Should().Be(31.50m);
        }
    }
}