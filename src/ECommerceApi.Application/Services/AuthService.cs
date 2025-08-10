using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.DTOs.Common;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Data;
using BC = BCrypt.Net.BCrypt;

namespace ECommerceApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

                if (user == null || !BC.Verify(loginDto.Password, user.PasswordHash))
                {
                    return ApiResponse<LoginResponseDto>.FailureResult("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    return ApiResponse<LoginResponseDto>.FailureResult("Account is deactivated");
                }

                // Update last login
                user.UpdateLastLogin();
                await _context.SaveChangesAsync();

                // Generate token
                var token = _tokenService.GenerateJwtToken(user);
                var userDto = _mapper.Map<UserDto>(user);

                var response = new LoginResponseDto
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = userDto
                };

                return ApiResponse<LoginResponseDto>.SuccessResult(response, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.FailureResult("An error occurred during login", ex.Message);
            }
        }

        // Other methods remain the same...
        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

                if (existingUser != null)
                {
                    return ApiResponse<UserDto>.FailureResult("User with this email already exists");
                }

                // Create new user
                var user = new User
                {
                    Email = registerDto.Email.ToLower(),
                    PasswordHash = BC.HashPassword(registerDto.Password),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Role = Domain.Enums.UserRole.Customer,
                    IsEmailConfirmed = false,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);
                return ApiResponse<UserDto>.SuccessResult(userDto, "User registered successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.FailureResult("An error occurred during registration", ex.Message);
            }
        }

        public async Task<ApiResponse<string>> LogoutAsync(int userId)
        {
            try
            {
                // In a real app, you might want to blacklist the token
                // For now, we'll just return success
                return ApiResponse<string>.SuccessResult("Logged out", "Logout successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailureResult("An error occurred during logout", ex.Message);
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserDto>.FailureResult("User not found");
                }

                var userDto = _mapper.Map<UserDto>(user);
                return ApiResponse<UserDto>.SuccessResult(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.FailureResult("An error occurred while fetching user profile", ex.Message);
            }
        }
    }
}