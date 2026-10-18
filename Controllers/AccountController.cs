using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TradingProject.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using TradingProject.Models;
using TradingProject.Models.Enums;

namespace TradingProject.Controllers
{
    public class AccountController(ApplicationDbContext context) : Controller
    {
        private async Task<bool> IsRegistrationEnabled()
        {
            var setting = await context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "RegistrationEnabled");
            return setting?.Value == "true";
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            if (!await IsRegistrationEnabled())
                return RedirectToAction(nameof(Login));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!await IsRegistrationEnabled())
                return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
                return View(model);

            if (await context.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", Resources.Resources.UsernameExists);
                return View(model);
            }

            var user = new ApplicationUser
            {
                Username = model.Username,
                DisplayName = model.DisplayName ?? model.Username,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = UserRole.Trader,
                RoleId = 2, // Default to Trader role
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Login), new { registered = true });
        }

        [HttpGet]
        public IActionResult Login(bool registered = false)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");
            
            ViewBag.Registered = registered;
            ViewBag.RegistrationEnabled = context.SystemSettings.Any(s => s.Key == "RegistrationEnabled" && s.Value == "true");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await context.Users
                .Include(u => u.AppRole)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                user.LastLoginDate = DateTime.UtcNow;
                await context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Role, user.AppRole?.Code ?? user.Role.ToString()),
                    new(ClaimTypes.Email, user.Email ?? string.Empty),
                    new("DisplayName", user.DisplayName ?? user.Username)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties { IsPersistent = true });

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), HttpOnly = true }
            );
            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }
            return View("GeneralError");
        }
    }
}
