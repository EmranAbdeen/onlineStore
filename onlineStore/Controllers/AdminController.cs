using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineStore.Data;
using onlineStore.Filters;
using onlineStore.Models;

namespace onlineStore.Controllers
{

    [AuthFilter("Admin")]
    public class AdminController : Controller
    {
        private readonly OnlineStoreDbContext _context;
        public AdminController(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Approve(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            user.IsApproved = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User '{user.FullName}' has been approved successfully.";
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Unapprove(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == id)
            {
                TempData["Error"] = "You cannot revoke approval for your own admin account!";
                return RedirectToAction("Users");
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            user.IsApproved = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Approval for '{user.FullName}' has been revoked.";
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Block(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == id)
            {
                TempData["Error"] = "You cannot block your own admin account!";
                return RedirectToAction("Users");
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            user.IsBlocked = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User '{user.FullName}' has been blocked.";
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Unblock(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            user.IsBlocked = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User '{user.FullName}' has been unblocked.";
            return RedirectToAction("Users");
        }

        //عندما تحذف الـ User، قاعدة البيانات ستحذف الـ Cart
        //المرتبطة به تلقائيًا.
        //  لا داعي تشغيل كود اللي عليهم كومنت بسبب جملة هاي في داتا بيس(ON DELETE CASCADE)
        public async Task<IActionResult> Delete(int id)
        {
            //var user = await _context.Users.Include(u => u.Cart).ThenInclude(c => c.CartItems).FirstOrDefaultAsync(u => u.Id == id);
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            //if(user.Cart != null)
            //{
            //    // حذف كل المنتجات الموجودة داخل سلة هذا المستخدم أولاً
            //    _context.CartItems.RemoveRange(user.Cart.CartItems);
            //    // حذف السلة نفسها 
            //    _context.Carts.Remove(user.Cart);
            //}

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User '{user.FullName}', Email '{user.Email}' has been deleted successfully.";
            return RedirectToAction("Users");
        }
    }
}
