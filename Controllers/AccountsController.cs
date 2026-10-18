using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Models;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class AccountsController(ApplicationDbContext context) : BaseController
    {
        private ApplicationDbContext Context { get; } = context;

        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.AccountsManage)) return Forbid();
            return View(await Context.TradingAccounts
                .Where(a => a.UserId == CurrentUserId)
                .Include(a => a.Broker)
                .OrderBy(a => a.Name)
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!HasPermission(PermissionCodes.AccountsManage)) return Forbid();
            ViewBag.Brokers = await Context.Brokers.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TradingAccount model)
        {
            if (!HasPermission(PermissionCodes.AccountsManage)) return Forbid();
            if (ModelState.IsValid)
            {
                model.UserId = CurrentUserId;
                model.CreatedDate = DateTime.UtcNow;
                Context.TradingAccounts.Add(model);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Brokers = await Context.Brokers.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.AccountsManage)) return Forbid();
            var account = await Context.TradingAccounts.FindAsync(id);
            if (account == null) return NotFound();
            
            // Check ownership
            if (!IsAdmin && account.UserId != CurrentUserId) return Forbid();

            ViewBag.Brokers = await Context.Brokers.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            return View("Create", account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TradingAccount model)
        {
            if (!HasPermission(PermissionCodes.AccountsManage)) return Forbid();
            if (id != model.Id) return BadRequest();

            var account = await Context.TradingAccounts.FindAsync(id);
            if (account == null) return NotFound();
            
            // Check ownership
            if (!IsAdmin && account.UserId != CurrentUserId) return Forbid();

            if (ModelState.IsValid)
            {
                account.Name = model.Name;
                account.AccountNumber = model.AccountNumber;
                account.BrokerId = model.BrokerId;
                account.IsDemo = model.IsDemo;
                account.Currency = model.Currency;
                account.IsActive = model.IsActive;

                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Brokers = await Context.Brokers.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasPermission(PermissionCodes.AccountsManage)) return Forbid();
            var account = await Context.TradingAccounts.FindAsync(id);
            if (account == null) return NotFound();
            
            // Check ownership
            if (!IsAdmin && account.UserId != CurrentUserId) return Forbid();

            Context.TradingAccounts.Remove(account);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
