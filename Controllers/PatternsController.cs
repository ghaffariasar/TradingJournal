using Microsoft.AspNetCore.Mvc;
using TradingProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class PatternsController(ApplicationDbContext context) : BaseController
    {
        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var patterns = await context.Patterns.ToListAsync();
            return View(patterns);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pattern model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.UtcNow;
                context.Patterns.Add(model);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var pattern = await context.Patterns.FindAsync(id);
            if (pattern == null) return NotFound();
            return View("Create", pattern);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pattern model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (id != model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                context.Patterns.Update(model);
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
            var pattern = await context.Patterns.FindAsync(id);
            if (pattern == null)
                return NotFound();
            context.Patterns.Remove(pattern);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
