using System;

namespace ECommerceApi.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Sku { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }

        public virtual User CreatedBy { get; set; } = null!;

        public bool IsInStock() => Stock > 0 && IsActive;

        public bool CanPurchase(int quantity) => IsInStock() && Stock >= quantity;

        public void UpdateStock(int newStock)
        {
            if (newStock < 0)
                throw new ArgumentException("Stock cannot be negative");

            Stock = newStock;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeductStock(int quantity)
        {
            if (!CanPurchase(quantity))
                throw new InvalidOperationException($"Cannot deduct {quantity} items. Available stock: {Stock}");

            Stock -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal GetTotalPrice(int quantity) => Price * quantity;
    }
}