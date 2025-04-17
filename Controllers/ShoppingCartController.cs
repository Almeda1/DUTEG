using DUTEG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DUTEG.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ShoppingCartController> _logger; // Add logger

        public ShoppingCartController(AppDbContext context, ILogger<ShoppingCartController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string GetUserIdentifier()
        {
            if (!HttpContext.Request.Cookies.TryGetValue("UserIdentifier", out var userId))
            {
                userId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append("UserIdentifier", userId);
                _logger.LogInformation("Created new UserIdentifier: {UserId}", userId);
            }
            return userId;
        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            _logger.LogInformation("AddToCart called for ProductId: {Id}", id);
            if (_context == null)
            {
                _logger.LogError("AppDbContext is null");
                return Json(new { success = false, message = "Database context is unavailable." });
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for Id: {Id}", id);
                return Json(new { success = false, message = "Product not found." });
            }

            var userId = GetUserIdentifier();
            var existingItem = _context.CartItems
                .FirstOrDefault(c => c.ProductId == id && c.UserIdentifier == userId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                _logger.LogInformation("Incremented quantity for CartItem: {ProductId}, User: {UserId}", id, userId);
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    ProductId = id,
                    Quantity = 1,
                    UserIdentifier = userId
                });
                _logger.LogInformation("Added new CartItem: {ProductId}, User: {UserId}", id, userId);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving cart changes for ProductId: {Id}", id);
                return Json(new { success = false, message = "Error updating cart." });
            }

            var totalCount = _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .Sum(c => c.Quantity);

            return Json(new { success = true, count = totalCount });
        }

        [HttpGet]
        public IActionResult ViewCart()
        {
            _logger.LogInformation("ViewCart called");
            if (_context == null)
            {
                _logger.LogError("AppDbContext is null");
                return StatusCode(500, "Database context is unavailable.");
            }

            var userId = GetUserIdentifier();
            var cart = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserIdentifier == userId)
                .ToList();

            return View(cart);
        }

        [HttpPost]
        public JsonResult UpdateCart(int id, int change)
        {
            _logger.LogInformation("UpdateCart called for ProductId: {Id}, Change: {Change}", id, change);
            if (_context == null)
            {
                _logger.LogError("AppDbContext is null");
                return Json(new { success = false, message = "Database context is unavailable." });
            }

            var userId = GetUserIdentifier();
            var item = _context.CartItems
                .FirstOrDefault(c => c.ProductId == id && c.UserIdentifier == userId);

            if (item != null)
            {
                item.Quantity += change;
                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                    _logger.LogInformation("Removed CartItem: {ProductId}, User: {UserId}", id, userId);
                }

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving cart update for ProductId: {Id}", id);
                    return Json(new { success = false, message = "Error updating cart." });
                }

                var cart = _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserIdentifier == userId)
                    .ToList();

                return Json(new
                {
                    success = true,
                    quantity = item.Quantity,
                    itemTotal = item.Product != null ? item.Product.Price * item.Quantity : 0,
                    total = cart.Sum(c => c.Product != null ? c.Product.Price * c.Quantity : 0)
                });
            }

            _logger.LogWarning("CartItem not found for ProductId: {Id}, User: {UserId}", id, userId);
            return Json(new { success = false, message = "Item not found in cart." });
        }

        [HttpPost]
        public JsonResult RemoveFromCart(int id)
        {
            _logger.LogInformation("RemoveFromCart called for ProductId: {Id}", id);
            if (_context == null)
            {
                _logger.LogError("AppDbContext is null");
                return Json(new { success = false, message = "Database context is unavailable." });
            }

            var userId = GetUserIdentifier();
            var item = _context.CartItems
                .FirstOrDefault(c => c.ProductId == id && c.UserIdentifier == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                try
                {
                    _context.SaveChanges();
                    _logger.LogInformation("Removed CartItem: {ProductId}, User: {UserId}", id, userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing CartItem for ProductId: {Id}", id);
                    return Json(new { success = false, message = "Error removing item." });
                }
            }

            var cart = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserIdentifier == userId)
                .ToList();

            return Json(new
            {
                success = true,
                count = cart.Sum(c => c.Quantity),
                total = cart.Sum(c => c.Product != null ? c.Product.Price * c.Quantity : 0)
            });
        }

        [HttpPost]
        public JsonResult ClearCart()
        {
            _logger.LogInformation("ClearCart called");
            if (_context == null)
            {
                _logger.LogError("AppDbContext is null");
                return Json(new { success = false, message = "Database context is unavailable." });
            }

            var userId = GetUserIdentifier();
            var items = _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .ToList();

            _context.CartItems.RemoveRange(items);
            try
            {
                _context.SaveChanges();
                _logger.LogInformation("Cleared cart for User: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for User: {UserId}", userId);
                return Json(new { success = false, message = "Error clearing cart." });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult GetCartCount()
        {
            _logger.LogInformation("GetCartCount called");
            if (_context == null)
            {
                _logger.LogError("AppDbContext is null");
                return Json(new { count = 0 });
            }

            var userId = GetUserIdentifier();
            var count = _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .Sum(c => c.Quantity);

            _logger.LogInformation("Cart count for User: {UserId} is {Count}", userId, count);
            return Json(new { count });
        }
    }
}