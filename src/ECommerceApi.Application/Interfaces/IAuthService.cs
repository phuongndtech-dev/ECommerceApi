using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.DTOs.Common;

namespace ECommerceApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<string>> LogoutAsync(int userId);
        Task<ApiResponse<UserDto>> GetUserProfileAsync(int userId);
    }
}