using DUTEG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DUTEG.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ShoppingCartController> _logger;

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
        public async Task<JsonResult> AddToCart(int id)
        {
            _logger.LogInformation("AddToCart called for ProductId: {Id}", id);

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                _logger.LogWarning("Product not found for Id: {Id}", id);
                return Json(new { success = false, message = "Product not found." });
            }

            var userId = GetUserIdentifier();
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == id && c.UserIdentifier == userId);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    ProductId = id,
                    Quantity = 1,
                    UserIdentifier = userId
                });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving cart changes for ProductId: {Id}", id);
                return Json(new { success = false, message = "Error updating cart." });
            }

            var count = await _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .SumAsync(c => c.Quantity);

            return Json(new { success = true, count });
        }

        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            _logger.LogInformation("ViewCart called");

            var userId = GetUserIdentifier();
            var cart = await _context.CartItems
                .AsNoTracking()
                .Include(c => c.Product)
                .Where(c => c.UserIdentifier == userId)
                .ToListAsync();

            return View(cart);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCart(int id, int change)
        {
            _logger.LogInformation("UpdateCart called for ProductId: {Id}, Change: {Change}", id, change);

            var userId = GetUserIdentifier();
            var item = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.ProductId == id && c.UserIdentifier == userId);

            if (item == null)
            {
                _logger.LogWarning("CartItem not found for ProductId: {Id}, User: {UserId}", id, userId);
                return Json(new { success = false, message = "Item not found in cart." });
            }

            item.Quantity += change;
            if (item.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart for ProductId: {Id}", id);
                return Json(new { success = false, message = "Error updating cart." });
            }

            var total = await _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .SumAsync(c => c.Product.Price * c.Quantity);

            return Json(new
            {
                success = true,
                quantity = item.Quantity,
                itemTotal = item.Product.Price * item.Quantity,
                total
            });
        }

        [HttpPost]
        public async Task<JsonResult> RemoveFromCart(int id)
        {
            _logger.LogInformation("RemoveFromCart called for ProductId: {Id}", id);

            var userId = GetUserIdentifier();
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == id && c.UserIdentifier == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing CartItem for ProductId: {Id}", id);
                    return Json(new { success = false, message = "Error removing item." });
                }
            }

            var cartData = await _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .Select(c => new { c.Quantity, c.Product.Price })
                .ToListAsync();

            var count = cartData.Sum(c => c.Quantity);
            var total = cartData.Sum(c => c.Price * c.Quantity);

            return Json(new { success = true, count, total });
        }

        [HttpPost]
        public async Task<JsonResult> ClearCart()
        {
            _logger.LogInformation("ClearCart called");

            var userId = GetUserIdentifier();
            var items = await _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for User: {UserId}", userId);
                return Json(new { success = false, message = "Error clearing cart." });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<JsonResult> GetCartCount()
        {
            _logger.LogInformation("GetCartCount called");

            var userId = GetUserIdentifier();
            var count = await _context.CartItems
                .Where(c => c.UserIdentifier == userId)
                .SumAsync(c => c.Quantity);

            return Json(new { count });
        }
    }
}
