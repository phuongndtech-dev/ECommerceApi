using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace ECommerceApi.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // DB has been seeded
            }

            // Seed Users
            var adminUser = new User
            {
                Email = "admin@ecommerce.com",
                PasswordHash = BC.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                IsEmailConfirmed = true,
                IsActive = true
            };

            var customerUser = new User
            {
                Email = "customer@example.com",
                PasswordHash = BC.HashPassword("Customer123!"),
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.Customer,
                IsEmailConfirmed = true,
                IsActive = true
            };

            await context.Users.AddRangeAsync(adminUser, customerUser);
            await context.SaveChangesAsync();

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    Name = "iPhone 15 Pro",
                    Description = "Latest iPhone with A17 Pro chip and titanium design",
                    ImageUrl = "https://images.unsplash.com/photo-1592750475338-74b7b21085ab",
                    Price = 999.99m,
                    Stock = 50,
                    Category = "Electronics",
                    Sku = "IPH15PRO001",
                    CreatedByUserId = adminUser.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "MacBook Pro M3",
                    Description = "14-inch MacBook Pro with M3 chip for professional workflows",
                    ImageUrl = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef",
                    Price = 1999.99m,
                    Stock = 25,
                    Category = "Electronics",
                    Sku = "MBP14M3001",
                    CreatedByUserId = adminUser.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "AirPods Pro 2",
                    Description = "Active Noise Cancellation wireless earbuds with spatial audio",
                    ImageUrl = "https://images.unsplash.com/photo-1572569511254-d8f925fe2cbb",
                    Price = 249.99m,
                    Stock = 100,
                    Category = "Electronics",
                    Sku = "APRO2001",
                    CreatedByUserId = adminUser.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Nike Air Max 270",
                    Description = "Comfortable running shoes with Air Max cushioning",
                    ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff",
                    Price = 149.99m,
                    Stock = 75,
                    Category = "Fashion",
                    Sku = "NIKAM270001",
                    CreatedByUserId = adminUser.Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Samsung 65\" QLED TV",
                    Description = "4K Ultra HD Smart TV with Quantum Dot technology",
                    ImageUrl = "https://images.unsplash.com/photo-1593359677879-a4bb92f829d1",
                    Price = 1299.99m,
                    Stock = 15,
                    Category = "Electronics",
                    Sku = "SAM65QLED001",
                    CreatedByUserId = adminUser.Id,
                    IsActive = true
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}