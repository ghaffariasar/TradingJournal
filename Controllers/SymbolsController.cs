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
    public class SymbolsController(ApplicationDbContext context) : BaseController
    {
        private ApplicationDbContext Context { get; } = context;

        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            return View(await Context.TradingSymbols.OrderBy(s => s.Symbol).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            ViewBag.InstrumentTypes = Enum.GetValues<InstrumentType>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TradingSymbol model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.UtcNow;
                Context.TradingSymbols.Add(model);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.InstrumentTypes = Enum.GetValues<InstrumentType>();
            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var symbol = await Context.TradingSymbols.FindAsync(id);
            if (symbol == null) return NotFound();
            ViewBag.InstrumentTypes = Enum.GetValues<InstrumentType>();
            return View("Create", symbol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TradingSymbol model)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            if (id != model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                Context.TradingSymbols.Update(model);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.InstrumentTypes = Enum.GetValues<InstrumentType>();
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasPermission(PermissionCodes.LookupsManage)) return Forbid();
            var symbol = await Context.TradingSymbols.FindAsync(id);
            if (symbol == null) 
                return NotFound();
            Context.TradingSymbols.Remove(symbol);

            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
