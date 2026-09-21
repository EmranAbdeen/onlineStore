using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineStore.Data;
using onlineStore.Filters;
using onlineStore.Models;

namespace onlineStore.Controllers
{
    [AuthFilter("Admin")]
    public class CategoryController : Controller
    {
        private readonly OnlineStoreDbContext _context;
        public CategoryController(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).ThenInclude(p => p.CartItems).FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            //Remove related products and their cart items first to avoid foreign key violations
            foreach (var product in category.Products.ToList())
            {
                if (product.CartItems?.Any() == true)
                    _context.CartItems.RemoveRange(product.CartItems);

                _context.Products.Remove(product);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category '{category.Name}' (and {category.Products.Count} linked products) has been deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}
