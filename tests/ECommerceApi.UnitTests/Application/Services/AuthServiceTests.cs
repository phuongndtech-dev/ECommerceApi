using AutoMapper;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using ECommerceApi.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ECommerceApi.UnitTests.Application.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _authService = new AuthService(_context, _tokenServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var userDto = new UserDto { Id = user.Id, Email = user.Email };
            var mockToken = "mock-jwt-token";

            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                       .Returns(userDto);
            _tokenServiceMock.Setup(t => t.GenerateJwtToken(It.IsAny<User>()))
                            .Returns(mockToken);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().Be(mockToken);
            result.Data.User.Should().Be(userDto);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFailure()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "wrongpassword"
            };

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid email or password");
        }

        [Fact]
        public async Task RegisterAsync_WithNewUser_ShouldReturnSuccess()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "newuser@example.com",
                Password = "password123",
                FirstName = "New",
                LastName = "User"
            };

            var userDto = new UserDto
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                       .Returns(userDto);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Email.Should().Be(registerDto.Email);

            // Verify user was added to database
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            userInDb.Should().NotBeNull();
            userInDb!.Role.Should().Be(UserRole.Customer);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
        {
            // Arrange
            var existingUser = new User
            {
                Email = "existing@example.com",
                PasswordHash = "hashedpassword"
            };

            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var registerDto = new RegisterDto
            {
                Email = "existing@example.com",
                Password = "password123",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("User with this email already exists");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
