using Microsoft.AspNetCore.Mvc;
using onlineStore.DTOs;
using onlineStore.Services.Interfaces;

namespace onlineStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.LoginAsync(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError("","Your account is blocked.");
                return View(model);
            }

            if (!user.IsApproved)
            {
                ModelState.AddModelError("","Your account is waiting for approval.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role.Name);
            HttpContext.Session.SetString("IsBlocked", user.IsBlocked.ToString());

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.RegisterAsync(model.FullName, model.Email, model.Password);

            if (!result)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            TempData["Success"] = "Registration successful. Wait for admin approval.";

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}
