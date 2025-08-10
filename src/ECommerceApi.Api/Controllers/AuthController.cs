using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.DTOs.Common;
using ECommerceApi.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        /// <returns>Created user information</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 400)]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterDto registerDto)
        {
            // Validate input
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<UserDto>.FailureResult("Validation failed", errors);
                return BadRequest(response);
            }

            var result = await _authService.RegisterAsync(registerDto);
            return HandleResult(result);
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 400)]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            // Validate input
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<LoginResponseDto>.FailureResult("Validation failed", errors);
                return BadRequest(response);
            }

            var result = await _authService.LoginAsync(loginDto);
            return HandleResult(result);
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<ActionResult<ApiResponse<string>>> Logout()
        {
            var userId = GetCurrentUserId();
            var result = await _authService.LogoutAsync(userId);
            return HandleResult(result);
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 401)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _authService.GetUserProfileAsync(userId);
            return HandleResult(result);
        }
    }
}
