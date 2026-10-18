using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Models;
using TradingProject.Models.Enums;
using TradingProject.Services;
using TradingProject.Helpers;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class UsersController(ApplicationDbContext context, IPermissionService permissionService)
        : BaseController
    {
        private ApplicationDbContext Context { get; } = context;

        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.UsersView)) return Forbid();
            var users = await Context.Users
                .Include(u => u.AppRole)
                .OrderBy(u => u.Username)
                .ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!HasPermission(PermissionCodes.UsersCreateEdit)) return Forbid();
            await PopulateCreateEditViewBags(null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser model, string password, int[] permissionIds)
        {
            if (!HasPermission(PermissionCodes.UsersCreateEdit)) return Forbid();
            
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Password must be at least 6 characters.");
                await PopulateCreateEditViewBags(null);
                return View("Create", model);
            }

            if (await Context.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "Username already exists.");
                await PopulateCreateEditViewBags(null);
                return View("Create", model);
            }

            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            model.CreatedDate = DateTime.UtcNow;
            Context.Users.Add(model);
            await Context.SaveChangesAsync();

            if (permissionIds != null && permissionIds.Length > 0)
            {
                await permissionService.ApplyUserOverridesAsync(model.Id, permissionIds);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionCodes.UsersCreateEdit)) return Forbid();
            var user = await Context.Users
                .Include(u => u.PermissionOverrides)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            
            await PopulateCreateEditViewBags(user);
            return View("Create", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplicationUser model, string? password, int[] permissionIds)
        {
            if (!HasPermission(PermissionCodes.UsersCreateEdit)) return Forbid();
            if (id != model.Id) return BadRequest();

            var user = await Context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (await Context.Users.AnyAsync(u => u.Username == model.Username && u.Id != id))
            {
                ModelState.AddModelError(nameof(model.Username), "Username already exists.");
                await PopulateCreateEditViewBags(user);
                return View("Create", model);
            }

            // Prevent removing the last active administrator
            if (user.RoleId == 1 && (model.RoleId != 1 || !model.IsActive))
            {
                var adminCount = await permissionService.CountActiveAdministratorsAsync();
                if (adminCount <= 1)
                {
                    ModelState.AddModelError(string.Empty, "Cannot deactivate or change role of the last active administrator.");
                    await PopulateCreateEditViewBags(user);
                    return View("Create", model);
                }
            }

            user.Username = model.Username;
            user.DisplayName = model.DisplayName;
            user.Email = model.Email;
            user.RoleId = model.RoleId;
            user.IsActive = model.IsActive;

            if (!string.IsNullOrWhiteSpace(password))
            {
                if (password.Length < 6)
                {
                    ModelState.AddModelError(string.Empty, "Password must be at least 6 characters.");
                    await PopulateCreateEditViewBags(user);
                    return View("Create", model);
                }
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            await Context.SaveChangesAsync();
            
            if (HasPermission(PermissionCodes.UsersManageAccess))
            {
                await permissionService.ApplyUserOverridesAsync(id, permissionIds ?? []);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCreateEditViewBags(ApplicationUser? user)
        {
            ViewBag.Roles = await Context.Roles.Where(r => r.IsActive).ToListAsync();
            ViewBag.Permissions = await Context.Permissions.OrderBy(p => p.Module).ToListAsync();
            ViewBag.UserPermissionIds = user?.PermissionOverrides?.Where(uo => uo.IsGranted).Select(uo => uo.PermissionId).ToList() ?? new List<int>();
        }

        [HttpGet]
        public async Task<IActionResult> UserReport(int id, DateTime? fromDate, DateTime? toDate, int? systemId, int? patternId, int? sessionId)
        {
            if (!HasPermission(PermissionCodes.UsersView)) return Forbid();
            var user = await Context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.TradSystems = await Context.TradSystems.Where(s => s.IsActive).ToListAsync();
            ViewBag.Patterns = await Context.Patterns.Where(p => p.IsActive).ToListAsync();
            ViewBag.Sessions = await Context.TradingSessions.Where(s => s.IsActive).ToListAsync();

            var model = new UserTradeReportViewModel
            {
                User = user,
                FromDate = fromDate,
                ToDate = toDate,
                SystemId = systemId,
                PatternId = patternId,
                SessionId = sessionId
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserReportData(int id, DateTime? fromDate, DateTime? toDate, int? systemId, int? patternId, int? sessionId, int start = 0, int length = 10)
        {
            if (!HasPermission(PermissionCodes.UsersView)) return Forbid();

            var query = Context.Trades
                .Where(t => t.UserId == id)
                .Include(t => t.TradingSession)
                .AsQueryable();

            if (fromDate.HasValue) query = query.Where(t => t.StartTime >= fromDate.Value);
            if (toDate.HasValue) 
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.StartTime <= endOfDay);
            }
            if (systemId.HasValue) query = query.Where(t => t.TradeTradeSystems.Any(j => j.TradSystemId == systemId.Value));
            if (patternId.HasValue) query = query.Where(t => t.TradeTradePatterns.Any(j => j.PatternId == patternId.Value));
            if (sessionId.HasValue) query = query.Where(t => t.TradingSessionId == sessionId.Value);

            var total = await query.CountAsync();
            var trades = await query
                .OrderByDescending(t => t.StartTime)
                .Skip(start)
                .Take(length)
                .ToListAsync();

            var data = trades.Select(t => new
            {
                t.Id,
                StartTime = t.StartTime,
                t.TradeSymbol,
                Direction = Resources.Resources.ResourceManager.GetString("Dir_" + t.Direction) ?? t.Direction.ToString(),
                Status = Resources.Resources.ResourceManager.GetString("Status_" + t.Status) ?? t.Status.ToString(),
                t.NetProfitLoss,
                t.RiskRewardRatio,
                Duration = TradeMetricsCalculator.FormatDuration(t.DurationMinutes)
            });

            // Also calculate metrics for the filtered set
            var allFilteredTrades = await query.ToListAsync();
            var metrics = TradeMetricsCalculator.CalculatePerformance(allFilteredTrades);

            return Json(new { recordsTotal = total, recordsFiltered = total, data, metrics });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!HasPermission(PermissionCodes.UsersCreateEdit)) return Forbid();
            if (id == CurrentUserId)
            {
                ModelState.AddModelError(string.Empty, Resources.Resources.CannotDeleteSelf);
                return RedirectToAction(nameof(Index));
            }

            var user = await Context.Users.FindAsync(id);
            if (user == null) return NotFound();

            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            if (!User.IsInRole("Admin")) return Forbid();

            var regEnabled = await Context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "RegistrationEnabled");
            if (regEnabled == null)
            {
                regEnabled = new SystemSetting { Key = "RegistrationEnabled", Value = "false", Description = Resources.Resources.RegistrationEnabledDescription };
                Context.SystemSettings.Add(regEnabled);
                await Context.SaveChangesAsync();
            }

            ViewBag.RegistrationEnabled = regEnabled.Value == "true";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistrationSetting(bool enabled)
        {
            if (!User.IsInRole("Admin")) return Forbid();

            var regEnabled = await Context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "RegistrationEnabled");
            if (regEnabled == null)
            {
                regEnabled = new SystemSetting { Key = "RegistrationEnabled", Value = enabled.ToString().ToLower(), Description = Resources.Resources.RegistrationEnabledDescription };
                Context.SystemSettings.Add(regEnabled);
            }
            else
            {
                regEnabled.Value = enabled.ToString().ToLower();
                regEnabled.LastUpdated = DateTime.UtcNow;
            }

            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Settings));
        }
    }
}
