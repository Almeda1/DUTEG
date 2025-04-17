using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DUTEG.Models
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            await CreateRolesAsync(roleManager);

            // Create admin user if it doesn't exist
            await CreateAdminUserAsync(userManager);

            // Seed products if the table is empty
            await SeedProductsAsync(context);
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateAdminUserAsync(UserManager<IdentityUser> userManager)
        {
            var adminUser = new IdentityUser
            {
                UserName = "admin@duteg.com",
                Email = "admin@duteg.com",
                EmailConfirmed = true
            };

            var existingAdmin = await userManager.FindByEmailAsync(adminUser.Email);
            if (existingAdmin == null)
            {
                var createResult = await userManager.CreateAsync(adminUser, "Admin@1234!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedProductsAsync(AppDbContext context)
        {
            // Check if there are any products already in the database
            if (!await context.Products.AnyAsync())
            {
                var products = new List<Product>
                {
                    new Product { Name = "BIG 6PCS GRANITE COOKWARE SET", Brand = "JIO", Price = 110000, ImageUrl = "/images/JIO.jpg", Category = "Pots" },
                    new Product { Name = "11PCS UCCLIFE POT", Brand = "UCCLIFE", Price = 140000, ImageUrl = "/images/S.POT1.jpeg", Category = "Pots" },
                    new Product { Name = "10PCS JIO POT", Brand = "JIO", Price = 115000, ImageUrl = "/images/S.POT2.jpeg", Category = "Pots" },
                    new Product { Name = "10PCS BOSCH POT", Brand = "BOSCH", Price = 85000, ImageUrl = "/images/S.POT3.jpeg", Category = "Pots" },
                    new Product { Name = "10PCS JIO SQUARE POT", Brand = "JIO", Price = 100000, ImageUrl = "/images/S.POT4.jpg", Category = "Pots" },
                    new Product { Name = "28cm STEAMER POT", Brand = "WIN WIN", Price = 21000, ImageUrl = "/images/STEAMER28.jpg", Category = "Pots" },
                    new Product { Name = "32cm STEAMER POT", Brand = "WIN WIN", Price = 32000, ImageUrl = "/images/STEAMER32.jpg", Category = "Pots" },
                    new Product { Name = "10PCS WHITE NON-STICK COOKWARE SET", Brand = "UCCLIFE", Price = 115000, ImageUrl = "/images/UCC2.jpg", Category = "Pots" },
                    new Product { Name = "8PCS WHITE GRANITE COOKWARE SET", Brand = "UCCLIFE", Price = 100000, ImageUrl = "/images/UCC.jpg", Category = "Pots" },
                    new Product { Name = "8PCS BLACK GRANITE COOKWARE SET", Brand = "UCCLIFE", Price = 100000, ImageUrl = "/images/UCC5.jpg", Category = "Pots" },
                    new Product { Name = "10PCS WHITE CERAMIC COATED COOKWARE SET", Brand = "UCCLIFE", Price = 115000, ImageUrl = "/images/UCC3.jpg", Category = "Pots" },
                    new Product { Name = "10PCS BLACK CERAMIC COATED COOKWARE SET", Brand = "UCCLIFE", Price = 115000, ImageUrl = "/images/UCC6.jpg", Category = "Pots" },
                    new Product { Name = "10PCS BLACK GRANITE COATED KITCHEN COOKWARE", Brand = "JIO", Price = 95000, ImageUrl = "/images/JIO2.jpg", Category = "Pots" },
                    new Product { Name = "10PCS GRANITE COATED KITCHEN COOKWARE", Brand = "JIO", Price = 95000, ImageUrl = "/images/JIO3.jpg", Category = "Pots" },
                    new Product { Name = "6PCS LOVESMILE COOKWARE SET", Brand = "LOVESMILE", Price = 105000, ImageUrl = "/images/LS1.jpg", Category = "Pots" },
                    new Product { Name = "10PCS LOVESMILE SQUARE COOKWARE SET", Brand = "LOVESMILE", Price = 110000, ImageUrl = "/images/LS2.jpg", Category = "Pots" },
                    new Product { Name = "10PCS BLACK LOVESMILE SQUARE COOKWARE SET", Brand = "LOVESMILE", Price = 110000, ImageUrl = "/images/LS3.jpg", Category = "Pots" },
                    new Product { Name = "Divine Dominion Air Fryer", Brand = "DIVINE DOMINION CRUSHER", Price = 40000, ImageUrl = "/images/AF1.jpg", Category = "AirFryers" },
                    new Product { Name = "Kenwood Air Fryer", Brand = "KENWOOD", Price = 46000, ImageUrl = "/images/AF2.jpg", Category = "AirFryers" },
                    new Product { Name = "Silver Crest Air Fryer", Brand = "SILVER CREST", Price = 44500, ImageUrl = "/images/AF3.jpg", Category = "AirFryers" }
                };

                try
                {
                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to seed products.", ex);
                }
            }
        }
    }
}