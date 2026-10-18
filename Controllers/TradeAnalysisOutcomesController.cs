using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Models;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class TradeAnalysisOutcomesController(ApplicationDbContext context) : BaseController
    {
        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var items = await context.TradeAnalysisOutcomes
                .OrderBy(o => o.SortOrder)
                .ThenBy(o => o.Name)
                .ToListAsync();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            return View(new TradeAnalysisOutcome { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TradeAnalysisOutcome model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedDate = DateTime.UtcNow;
            context.TradeAnalysisOutcomes.Add(model);
            await context.SaveChangesAsync();
            TempData["Success"] = Resources.Resources.SaveSuccess;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var item = await context.TradeAnalysisOutcomes.FindAsync(id);
            if (item == null) return NotFound();
            return View("Create", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TradeAnalysisOutcome model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
                return View("Create", model);

            context.TradeAnalysisOutcomes.Update(model);
            await context.SaveChangesAsync();
            TempData["Success"] = Resources.Resources.SaveSuccess;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var item = await context.TradeAnalysisOutcomes.FindAsync(id);
            if (item == null) return NotFound();
            context.TradeAnalysisOutcomes.Remove(item);
            await context.SaveChangesAsync();
            TempData["Success"] = Resources.Resources.DeleteSuccess;
            return RedirectToAction(nameof(Index));
        }
    }
}
