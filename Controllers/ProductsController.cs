using DUTEG.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace DUTEG.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public IActionResult Pots()
        {
            var products = _context.Products
                .Where(p => p.Category == "Pots")
                .ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult AirFryers()
        {
            var products = _context.Products
                .Where(p => p.Category == "AirFryers")
                .ToList();
            return View(products);
        }

        public IActionResult PressureCookers()
        {
            var products = _context.Products
                .Where(p => p.Category == "Pressure Cookers")
                .ToList();
            return View(products);
        }

        public IActionResult Blenders()
        {
            var products = _context.Products
                .Where(p => p.Category == "Blenders")
                .ToList();
            return View(products);
        }

        public IActionResult DishRacks()
        {
            var products = _context.Products
                .Where(p => p.Category == "Dish Racks")
                .ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Products.Add(product);
                    _context.SaveChanges();
                    return RedirectToAction(product.Category == "Pots" ? "Pots" : "AirFryers");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while saving the product.");
                }
            }
            return View(product);
        }
    }
}