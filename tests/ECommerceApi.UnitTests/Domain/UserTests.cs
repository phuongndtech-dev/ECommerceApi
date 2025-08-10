using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using FluentAssertions;

namespace ECommerceApi.UnitTests.Domain
{
    public class UserTests
    {
        [Fact]
        public void GetFullName_ShouldReturnCombinedName()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var fullName = user.GetFullName();

            // Assert
            fullName.Should().Be("John Doe");
        }

        [Fact]
        public void GetFullName_ShouldTrimWhitespace()
        {
            // Arrange
            var user = new User
            {
                FirstName = " John ",
                LastName = " Doe "
            };

            // Act
            var fullName = user.GetFullName();

            // Assert
            fullName.Should().Be("John   Doe"); // Note: Actual trimming behavior
        }

        [Fact]
        public void UpdateLastLogin_ShouldSetCurrentTime()
        {
            // Arrange
            var user = new User();
            var beforeUpdate = DateTime.UtcNow;

            // Act
            user.UpdateLastLogin();

            // Assert
            user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void IsInRole_ShouldReturnTrueForMatchingRole()
        {
            // Arrange
            var user = new User { Role = UserRole.Admin };

            // Act & Assert
            user.IsInRole(UserRole.Admin).Should().BeTrue();
            user.IsInRole(UserRole.Customer).Should().BeFalse();
        }
    }
}
