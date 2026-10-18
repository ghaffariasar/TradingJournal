using Microsoft.AspNetCore.Mvc;
using TradingProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class TradSystemsController(ApplicationDbContext context) : BaseController
    {
        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var systems = await context.TradSystems.ToListAsync();
            return View(systems);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TradSystem model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.UtcNow;
                context.TradSystems.Add(model);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var system = await context.TradSystems.FindAsync(id);
            if (system == null) return NotFound();
            return View("Create", system);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TradSystem model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                context.TradSystems.Update(model);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var system = await context.TradSystems.FindAsync(id);
            if (system == null)
                return NotFound();
            context.TradSystems.Remove(system);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
