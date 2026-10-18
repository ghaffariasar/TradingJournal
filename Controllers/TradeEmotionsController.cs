using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Models;
using TradingProject.Models.Enums;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class TradeEmotionsController(ApplicationDbContext context) : BaseController
    {
        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var items = await context.TradeEmotions
                .OrderBy(e => e.Category)
                .ThenBy(e => e.SortOrder)
                .ThenBy(e => e.Name)
                .ToListAsync();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            PopulateCategories();
            return View(new TradeEmotion { IsActive = true, Category = EmotionCategory.AtEntry });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TradeEmotion model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (!ModelState.IsValid)
            {
                PopulateCategories();
                return View(model);
            }

            model.CreatedDate = DateTime.UtcNow;
            context.TradeEmotions.Add(model);
            await context.SaveChangesAsync();
            TempData["Success"] = Resources.Resources.SaveSuccess;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var item = await context.TradeEmotions.FindAsync(id);
            if (item == null) return NotFound();
            PopulateCategories();
            return View("Create", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TradeEmotion model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                PopulateCategories();
                return View("Create", model);
            }

            context.TradeEmotions.Update(model);
            await context.SaveChangesAsync();
            TempData["Success"] = Resources.Resources.SaveSuccess;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var item = await context.TradeEmotions.FindAsync(id);
            if (item == null) return NotFound();
            context.TradeEmotions.Remove(item);
            await context.SaveChangesAsync();
            TempData["Success"] = Resources.Resources.DeleteSuccess;
            return RedirectToAction(nameof(Index));
        }

        private void PopulateCategories() =>
            ViewBag.Categories = Enum.GetValues<EmotionCategory>();
    }
}
