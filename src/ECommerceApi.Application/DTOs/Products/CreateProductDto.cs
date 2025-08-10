﻿using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Application.DTOs.Products
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Url]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Sku { get; set; } = string.Empty;
    }
}
