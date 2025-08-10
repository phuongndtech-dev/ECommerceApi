using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Application.DTOs.Common;

namespace ECommerceApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult<ApiResponse<T>> HandleResult<T>(ApiResponse<T> result)
        {
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId") ??
                             User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User ID not found in token");
        }

        protected string GetCurrentUserEmail()
        {
            var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email);
            return emailClaim?.Value ?? throw new UnauthorizedAccessException("Email not found in token");
        }
    }
}
