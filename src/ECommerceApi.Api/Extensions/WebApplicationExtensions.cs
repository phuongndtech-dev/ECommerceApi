using ECommerceApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> ApplyMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                if ((await context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await context.Database.MigrateAsync();
                    Console.WriteLine("Database migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("No pending migrations found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
                throw;
            }

            return app;
        }
    }
}
