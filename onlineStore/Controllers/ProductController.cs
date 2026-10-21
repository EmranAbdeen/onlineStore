using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineStore.Data;
using onlineStore.Filters;
using onlineStore.Models;

namespace onlineStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly OnlineStoreDbContext _context;
        public ProductController(OnlineStoreDbContext context)
        {
            _context = context;
        }

        // User can browse products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return View(products);
        }

        // Admin only
        [AuthFilter("Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [AuthFilter("Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ModelState.Remove(nameof(Product.Category));
            ModelState.Remove(nameof(Product.CartItems));
            ModelState.Remove(nameof(Product.ImageFile));

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }

            if (product.ImageFile != null)
            {
                Directory.CreateDirectory("wwwroot/images");

                var fileName = Guid.NewGuid().ToString() + Path.GetFileName(product.ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    product.ImageFile.CopyTo(stream);
                }

                product.ImageUrl = $"/images/{fileName}";
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // Admin only
        [AuthFilter("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.CartItems).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            // Remove related cart items to avoid foreign key violation
            if (product.CartItems.Any())
            {
                _context.CartItems.RemoveRange(product.CartItems);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' has been deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
