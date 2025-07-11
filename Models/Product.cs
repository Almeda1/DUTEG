﻿namespace DUTEG.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; } // Nullable if some products might not have brands
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1; // Default quantity

        // Add any additional properties you need
    }
}
