using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineStore.Data;
using onlineStore.Models;

namespace onlineStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly OnlineStoreDbContext _context;
        public HomeController(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return View(products);
        }
    }
}
