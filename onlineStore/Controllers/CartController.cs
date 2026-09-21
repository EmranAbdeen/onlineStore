using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineStore.Data;
using onlineStore.DTOs;
using onlineStore.Filters;
using onlineStore.Models;

namespace onlineStore.Controllers
{
    [AuthFilter]
    public class CartController : Controller
    {
        private readonly OnlineStoreDbContext _context;
        public CartController(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var cart = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return View(cart);
        }

        public async Task<IActionResult> Add(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cart = await GetOrCreateCartAsync(userId.Value);
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int itemId)
        {
            var item = await GetUserCartItemAsync(itemId);
            if (item == null)
                return RedirectToAction("Index");

            item.Quantity++;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Decrease(int itemId)
        {
            var item = await GetUserCartItemAsync(itemId);
            if (item == null)
                return RedirectToAction("Index");

            if (item.Quantity <= 1)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity--;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // بدل ما تعمل (Increase,Decrease)
        // بل ازرار تعمل عن طريق ال input
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity) 
        {
            var item = await GetUserCartItemAsync(itemId);
            if (item == null)
                return RedirectToAction("Index");

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int itemId)
        {
            var item = await GetUserCartItemAsync(itemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
                return cart;

            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        private async Task<CartItem?> GetUserCartItemAsync(int itemId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return null;

            var item = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == userId.Value);

            return item;
        }

    }
}
