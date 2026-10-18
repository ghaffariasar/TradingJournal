using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Models;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class BrokersController(ApplicationDbContext context) : BaseController
    {
        private ApplicationDbContext Context { get; } = context;

        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            return View(await Context.Brokers.OrderBy(b => b.Name).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Broker model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.UtcNow;
                Context.Brokers.Add(model);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var broker = await Context.Brokers.FindAsync(id);
            if (broker == null) return NotFound();
            return View("Create", broker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Broker model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (id != model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                Context.Brokers.Update(model);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var broker = await Context.Brokers.FindAsync(id);
            if (broker == null) return NotFound();
            Context.Brokers.Remove(broker);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
