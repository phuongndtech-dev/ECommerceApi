using ECommerceApi.Domain.Entities;
using System.Security.Claims;

namespace ECommerceApi.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        string? GetEmailFromToken(string token);
    }
}