using ECommerceApi.Application.DTOs.Common;
using ECommerceApi.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ECommerceApi.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                NotFoundException => new ApiResponse<object>
                {
                    Success = false,
                    Message = exception.Message,
                    Errors = new List<string> { exception.Message }
                },
                ValidationException validationEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationEx.Errors.SelectMany(x => x.Value).ToList()
                },
                DomainException => new ApiResponse<object>
                {
                    Success = false,
                    Message = exception.Message,
                    Errors = new List<string> { exception.Message }
                },
                _ => new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while processing your request",
                    Errors = new List<string> { "Internal server error" }
                }
            };

            context.Response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationException => (int)HttpStatusCode.BadRequest,
                DomainException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}