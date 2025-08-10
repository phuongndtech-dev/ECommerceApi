using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.Validators;
using FluentAssertions;

namespace ECommerceApi.UnitTests.Application.Validators
{
    public class RegisterDtoValidatorTests
    {
        private readonly RegisterDtoValidator _validator;

        public RegisterDtoValidatorTests()
        {
            _validator = new RegisterDtoValidator();
        }

        [Fact]
        public void Validate_WithValidData_ShouldPass()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@example.com")]
        public void Validate_WithInvalidEmail_ShouldFail(string email)
        {
            // Arrange
            var dto = new RegisterDto
            {
                Email = email,
                Password = "Password123",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.Email));
        }

        [Theory]
        [InlineData("12345")]     // Too short
        [InlineData("password")]  // No uppercase
        [InlineData("PASSWORD")]  // No lowercase
        [InlineData("Password")]  // No digit
        public void Validate_WithInvalidPassword_ShouldFail(string password)
        {
            // Arrange
            var dto = new RegisterDto
            {
                Email = "test@example.com",
                Password = password,
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.Password));
        }

        [Theory]
        [InlineData("")]
        [InlineData("John123")]    // Contains numbers
        [InlineData("John@Doe")]   // Contains special chars
        public void Validate_WithInvalidName_ShouldFail(string name)
        {
            // Arrange
            var dto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123",
                FirstName = name,
                LastName = "Doe"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.FirstName));
        }
    }
}