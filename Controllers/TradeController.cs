using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using TradingProject.Data;
using TradingProject.Helpers;
using TradingProject.Models;
using TradingProject.Models.Enums;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class TradesController(ApplicationDbContext context) : BaseController
    {
        private static readonly string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        private ApplicationDbContext Context { get; } = context;

        public async Task<IActionResult> Index()
        {
            if (!HasPermission(PermissionCodes.TradesViewOwn) && !HasPermission(PermissionCodes.UsersViewOthersTrades))
                return Forbid();

            ViewBag.TradSystems = await Context.TradSystems.Where(s => s.IsActive).OrderBy(s => s.SystemName).ToListAsync();
            ViewBag.Patterns = await Context.Patterns.Where(p => p.IsActive).OrderBy(p => p.PatternName).ToListAsync();
            ViewBag.Sessions = await Context.TradingSessions.Where(s => s.IsActive).OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToListAsync();
            
            if (HasPermission(PermissionCodes.UsersViewOthersTrades))
            {
                ViewBag.Users = await Context.Users.Where(u => u.IsActive).OrderBy(u => u.DisplayName).ToListAsync();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTrades(string? search, string? symbol, string? orderNumber, TradeStatus? status, TradeDirection? direction, bool? isDemo, int? systemId, int? patternId, int? sessionId, int? userId, DateTime? fromDate, DateTime? toDate, int start = 0, int length = 25, string sortColumn = "Id", string sortDir = "desc")
        {
            if (!HasPermission(PermissionCodes.TradesViewOwn) && !HasPermission(PermissionCodes.UsersViewOthersTrades))
                return Forbid();

            var query = Context.Trades
                .Include(t => t.Symbol)
                .Include(t => t.Broker)
                .Include(t => t.TradingAccount)
                .Include(t => t.TradingSession)
                .Include(t => t.User)
                .Include(t => t.TradeTradeSystems).ThenInclude(j => j.TradSystem)
                .Include(t => t.TradeTradePatterns).ThenInclude(j => j.Pattern)
                .AsQueryable();

            if (!HasPermission(PermissionCodes.UsersViewOthersTrades))
            {
                query = query.Where(t => t.UserId == CurrentUserId);
            }
            else if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.OrderNumber.Contains(search) || t.TradeSymbol.Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(symbol))
                query = query.Where(t => t.TradeSymbol.Contains(symbol));
            if (!string.IsNullOrWhiteSpace(orderNumber))
                query = query.Where(t => t.OrderNumber.Contains(orderNumber));
            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);
            if (direction.HasValue)
                query = query.Where(t => t.Direction == direction.Value);
            if (isDemo.HasValue)
                query = query.Where(t => t.IsDemoAccount == isDemo.Value);
            if (systemId.HasValue)
                query = query.Where(t => t.TradeTradeSystems.Any(j => j.TradSystemId == systemId.Value));
            if (patternId.HasValue)
                query = query.Where(t => t.TradeTradePatterns.Any(j => j.PatternId == patternId.Value));
            if (sessionId.HasValue)
                query = query.Where(t => t.TradingSessionId == sessionId.Value);
            if (fromDate.HasValue)
                query = query.Where(t => t.StartTime >= fromDate.Value);
            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.StartTime <= endOfDay);
            }

            var total = await query.CountAsync();

            query = sortColumn?.ToLower() switch
            {
                "starttime" => sortDir == "asc" ? query.OrderBy(t => t.StartTime) : query.OrderByDescending(t => t.StartTime),
                "symbol" => sortDir == "asc" ? query.OrderBy(t => t.TradeSymbol) : query.OrderByDescending(t => t.TradeSymbol),
                "realizedpnl" or "netpnl" => sortDir == "asc" ? query.OrderBy(t => t.NetProfitLoss) : query.OrderByDescending(t => t.NetProfitLoss),
                "riskrewardratio" => sortDir == "asc" ? query.OrderBy(t => t.RiskRewardRatio) : query.OrderByDescending(t => t.RiskRewardRatio),
                "durationminutes" => sortDir == "asc" ? query.OrderBy(t => t.DurationMinutes) : query.OrderByDescending(t => t.DurationMinutes),
                _ => sortDir == "asc" ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Id),
            };

            var trades = await query
                .Skip(start)
                .Take(length)
                .ToListAsync();

            var data = trades.Select(t => new
            {
                t.Id,
                t.OrderNumber,
                t.TradeSymbol,
                Session = t.TradingSession == null ? null : LocaleHelper.Pick(t.TradingSession.Name, t.TradingSession.NameFa),
                DirectionKey = t.Direction.ToString(),
                Direction = Resources.Resources.ResourceManager.GetString("Dir_" + t.Direction) ?? t.Direction.ToString(),
                StatusKey = t.Status.ToString(),
                Status = Resources.Resources.ResourceManager.GetString("Status_" + t.Status) ?? t.Status.ToString(),
                IsDemo = t.IsDemoAccount,
                StartTime = t.StartTime,
                RealizedPnL = t.RealizedPnL,
                NetPnL = t.NetPnL,
                RiskRewardRatio = t.RiskRewardRatio,
                DurationMinutes = t.DurationMinutes,
                Duration = TradeMetricsCalculator.FormatDuration(t.DurationMinutes),
                Systems = string.Join(", ", t.TradeTradeSystems.Select(j => j.TradSystem?.SystemName)),
                Patterns = string.Join(", ", t.TradeTradePatterns.Select(j => j.Pattern?.PatternName))
            });

            return Json(new { recordsTotal = total, recordsFiltered = total, data });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!HasPermission(PermissionCodes.TradesCreateOwn))
                return Forbid();

            await PopulateViewBags();
            return View(new Trade { StartTime = DateTime.UtcNow, Status = TradeStatus.Open, Direction = TradeDirection.Long });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trade model, int[] selectedSystemIds, int[] selectedPatternIds, int[] selectedEmotionIds, List<IFormFile>? attachments, string? submitAction)
        {
            if (!HasPermission(PermissionCodes.TradesCreateOwn))
                return Forbid();

            if (string.IsNullOrWhiteSpace(model.TradeSymbol) && model.SymbolId.HasValue)
            {
                var sym = await Context.TradingSymbols.FirstOrDefaultAsync(s => s.Id == model.SymbolId.Value);
                if (sym != null) model.TradeSymbol = sym.Symbol;
            }

            if (model.StartTime != default)
                model.StartTime = DateTime.SpecifyKind(model.StartTime, DateTimeKind.Local);
            if (model.EndTime.HasValue)
                model.EndTime = DateTime.SpecifyKind(model.EndTime.Value, DateTimeKind.Local);

            if (!ModelState.IsValid)
            {
                ViewBag.PreselectedSystemIds = selectedSystemIds?.ToList() ?? [];
                ViewBag.PreselectedPatternIds = selectedPatternIds?.ToList() ?? [];
                ViewBag.PreselectedEmotionIds = selectedEmotionIds?.ToList() ?? [];
                await PopulateViewBags();
                return View(model);
            }

            try
            {
                model.UserId = CurrentUserId;
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = null;

                if (model.Timeframe.HasValue)
                    model.Timeframe = model.Timeframe.Value;

                TradeMetricsCalculator.Apply(model);
                await ApplySystemsAndPatterns(model, selectedSystemIds, selectedPatternIds);
                await ApplyEmotions(model, selectedEmotionIds);
                await SaveAttachments(model, attachments);

                Context.Trades.Add(model);
                await Context.SaveChangesAsync();

                TempData["Success"] = Resources.Resources.SaveSuccess;

                return submitAction switch
                {
                    "SaveNew" => RedirectToAction(nameof(Create)),
                    "SaveCopy" => RedirectToAction(nameof(Copy), new { id = model.Id }),
                    _ => RedirectToAction(nameof(Index))
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["Error"] = Resources.Resources.OperationError;
                await PopulateViewBags();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Copy(int id)
        {
            var trade = await Context.Trades
                .Include(t => t.TradeTradeSystems)
                .Include(t => t.TradeTradePatterns)
                .Include(t => t.TradeTradeEmotions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trade == null)
                return NotFound();

            if (trade.UserId != CurrentUserId && !HasPermission(PermissionCodes.UsersViewOthersTrades))
                return Forbid();

            if (!HasPermission(PermissionCodes.TradesCreateOwn))
                return Forbid();

            var copy = new Trade
            {
                SymbolId = trade.SymbolId,
                TradeSymbol = trade.TradeSymbol,
                BrokerId = trade.BrokerId,
                TradingAccountId = trade.TradingAccountId,
                TradingSessionId = trade.TradingSessionId,
                TradeAnalysisOutcomeId = trade.TradeAnalysisOutcomeId,
                Direction = trade.Direction,
                Timeframe = trade.Timeframe,
                LotSize = trade.LotSize,
                StopLoss = trade.StopLoss,
                TakeProfit = trade.TakeProfit,
                TrendConfirmation1M = trade.TrendConfirmation1M,
                TrendConfirmation5M = trade.TrendConfirmation5M,
                TrendConfirmation15M = trade.TrendConfirmation15M,
                DivergenceHD = trade.DivergenceHD,
                DivergenceRD = trade.DivergenceRD,
                EOC = trade.EOC,
                EOW = trade.EOW,
                IsDemoAccount = trade.IsDemoAccount,
                Notes = trade.Notes,
                OrderNumber = string.Empty,
                StartTime = DateTime.UtcNow,
                EndTime = null,
                EntryPrice = 0,
                ExitPrice = null,
                RealizedPnL = null,
                Fees = null,
                RiskAmount = null,
                ExitReason = null,
                AccountBalance = null,
                IsPass = false,
                Status = TradeStatus.Open
            };

            ViewBag.PreselectedSystemIds = trade.TradeTradeSystems.Select(j => j.TradSystemId).ToList();
            ViewBag.PreselectedPatternIds = trade.TradeTradePatterns.Select(j => j.PatternId).ToList();
            ViewBag.PreselectedEmotionIds = trade.TradeTradeEmotions.Select(j => j.TradeEmotionId).ToList();

            await PopulateViewBags();
            return View(nameof(Create), copy);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trade = await Context.Trades
                .Include(t => t.TradeTradeSystems)
                .Include(t => t.TradeTradePatterns)
                .Include(t => t.TradeTradeEmotions)
                .Include(t => t.Attachments)
                .Include(t => t.Symbol)
                .Include(t => t.Broker)
                .Include(t => t.TradingAccount)
                .Include(t => t.TradingSession)
                .Include(t => t.TradeAnalysisOutcome)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trade == null)
                return NotFound();

            if (trade.UserId != CurrentUserId && !IsAdmin)
                return Forbid();

            if (!HasPermission(PermissionCodes.TradesEditOwn) && !IsAdmin)
                return Forbid();

            ViewBag.PreselectedSystemIds = trade.TradeTradeSystems.Select(j => j.TradSystemId).ToList();
            ViewBag.PreselectedPatternIds = trade.TradeTradePatterns.Select(j => j.PatternId).ToList();
            ViewBag.PreselectedEmotionIds = trade.TradeTradeEmotions.Select(j => j.TradeEmotionId).ToList();

            await PopulateViewBags();
            return View(trade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trade model, int[] selectedSystemIds, int[] selectedPatternIds, int[] selectedEmotionIds, List<IFormFile>? attachments, string? submitAction)
        {
            if (id != model.Id)
                return BadRequest();

            var trade = await Context.Trades
                .Include(t => t.TradeTradeSystems)
                .Include(t => t.TradeTradePatterns)
                .Include(t => t.TradeTradeEmotions)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trade == null)
                return NotFound();

            if (trade.UserId != CurrentUserId && !IsAdmin)
                return Forbid();

            if (!HasPermission(PermissionCodes.TradesEditOwn) && !IsAdmin)
                return Forbid();

            if (string.IsNullOrWhiteSpace(model.TradeSymbol) && model.SymbolId.HasValue)
            {
                var sym = await Context.TradingSymbols.FirstOrDefaultAsync(s => s.Id == model.SymbolId.Value);
                if (sym != null) model.TradeSymbol = sym.Symbol;
            }
            if (model.StartTime != default)
                model.StartTime = DateTime.SpecifyKind(model.StartTime, DateTimeKind.Local);
            if (model.EndTime.HasValue)
                model.EndTime = DateTime.SpecifyKind(model.EndTime.Value, DateTimeKind.Local);

            if (!ModelState.IsValid)
            {
                ViewBag.PreselectedSystemIds = selectedSystemIds.ToList();
                ViewBag.PreselectedPatternIds = selectedPatternIds.ToList();
                ViewBag.PreselectedEmotionIds = selectedEmotionIds?.ToList() ?? [];
                await PopulateViewBags();
                return View(model);
            }

            try
            {
                trade.OrderNumber = model.OrderNumber;
                trade.SymbolId = model.SymbolId;
                trade.TradeSymbol = model.TradeSymbol;
                trade.BrokerId = model.BrokerId;
                trade.TradingAccountId = model.TradingAccountId;
                trade.TradingSessionId = model.TradingSessionId;
                trade.TradeAnalysisOutcomeId = model.TradeAnalysisOutcomeId;
                trade.Direction = model.Direction;
                trade.Status = model.Status;
                trade.Timeframe = model.Timeframe;
                trade.StartTime = model.StartTime;
                trade.EndTime = model.EndTime;
                trade.AccountBalance = model.AccountBalance;
                trade.LotSize = model.LotSize;
                trade.EntryPrice = model.EntryPrice;
                trade.ExitPrice = model.ExitPrice;
                trade.TakeProfit = model.TakeProfit;
                trade.StopLoss = model.StopLoss;
                trade.RealizedPnL = model.RealizedPnL;
                trade.Fees = model.Fees;
                trade.RiskAmount = model.RiskAmount;
                trade.ExitReason = model.ExitReason;
                trade.Notes = model.Notes;
                trade.TrendConfirmation1M = model.TrendConfirmation1M;
                trade.TrendConfirmation5M = model.TrendConfirmation5M;
                trade.TrendConfirmation15M = model.TrendConfirmation15M;
                trade.DivergenceHD = model.DivergenceHD;
                trade.DivergenceRD = model.DivergenceRD;
                trade.EOC = model.EOC;
                trade.EOW = model.EOW;
                trade.IsPass = model.IsPass;
                trade.IsDemoAccount = model.IsDemoAccount;
                trade.UpdatedDate = DateTime.UtcNow;
                TradeMetricsCalculator.Apply(trade);

                await ApplySystemsAndPatterns(trade, selectedSystemIds, selectedPatternIds, true);
                await ApplyEmotions(trade, selectedEmotionIds, true);
                await SaveAttachments(trade, attachments, true);

                await Context.SaveChangesAsync();

                TempData["Success"] = Resources.Resources.SaveSuccess;

                return submitAction == "SaveNew"
                    ? RedirectToAction(nameof(Create))
                    : RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["Error"] = Resources.Resources.OperationError;
                ViewBag.PreselectedSystemIds = selectedSystemIds.ToList();
                ViewBag.PreselectedPatternIds = selectedPatternIds.ToList();
                ViewBag.PreselectedEmotionIds = selectedEmotionIds?.ToList() ?? [];
                await PopulateViewBags();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var trade = await Context.Trades.FindAsync(id);
            if (trade == null)
                return NotFound();

            if (trade.UserId != CurrentUserId && !IsAdmin)
                return Forbid();

            if (!HasPermission(PermissionCodes.TradesDeleteOwn) && !IsAdmin)
                return Forbid();

            Context.Trades.Remove(trade);
            await Context.SaveChangesAsync();
            TempData["Success"] = Resources.Resources.DeleteSuccess;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var trade = await Context.Trades
                .Include(t => t.Symbol)
                .Include(t => t.Broker)
                .Include(t => t.TradingAccount)
                .Include(t => t.TradingSession)
                .Include(t => t.TradeAnalysisOutcome)
                .Include(t => t.TradeTradeSystems).ThenInclude(j => j.TradSystem)
                .Include(t => t.TradeTradePatterns).ThenInclude(j => j.Pattern)
                .Include(t => t.TradeTradeEmotions).ThenInclude(j => j.TradeEmotion)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trade == null)
                return NotFound();

            if (trade.UserId != CurrentUserId && !HasPermission(PermissionCodes.UsersViewOthersTrades))
                return Forbid();

            if (!HasPermission(PermissionCodes.TradesViewOwn) && !HasPermission(PermissionCodes.UsersViewOthersTrades))
                return Forbid();

            return View(trade);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var attachment = await Context.TradeAttachments.FindAsync(id);
            if (attachment == null)
                return NotFound();

            var trade = await Context.Trades.FindAsync(attachment.TradeId);
            if (trade == null)
                return NotFound();

            if (!IsAdmin && trade.UserId != CurrentUserId)
                return Forbid();

            var fullPath = Path.IsPathRooted(attachment.FilePath)
                ? attachment.FilePath
                : Path.Combine(BasePath, attachment.FilePath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            const string defaultContentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out var contentType))
                contentType = defaultContentType;

            return PhysicalFile(fullPath, contentType, Path.GetFileName(fullPath));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var attachment = await Context.TradeAttachments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attachment == null)
                return NotFound();

            var trade = await Context.Trades
                .FirstOrDefaultAsync(t => t.Id == attachment.TradeId);

            if (trade == null)
                return NotFound();

            if (!IsAdmin && trade.UserId != CurrentUserId)
                return Forbid();

            try
            {
                // Delete the physical file if it exists.
                DeleteAttachmentFile(attachment.FilePath);

                // Delete the database record.
                Context.TradeAttachments.Remove(attachment);
                await Context.SaveChangesAsync();

                TempData["Success"] = Resources.Resources.SaveSuccess;
            }
            catch (Exception)
            {
                TempData["Error"] = Resources.Resources.OperationError;
            }

            return RedirectToAction(nameof(Edit), new { id = attachment.TradeId });
        }


        #region Private Methods

        private async Task PopulateViewBags()
        {
            ViewBag.Symbols  = await Context.TradingSymbols
                .Where(s => s.IsActive)
                .OrderBy(s => s.Symbol)
                .Select(p => new
                {
                    p.Id,
                    p.Symbol,
                    p.Name,
                    p.NameFa,
                    p.InstrumentType,
                    Instrument = p.InstrumentType.ToString()
                })
                .ToListAsync();
            ViewBag.Brokers = await Context.Brokers.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();
            ViewBag.Accounts = await Context.TradingAccounts
                .Where(a => a.IsActive && (a.UserId == CurrentUserId))
                .OrderBy(a => a.Name)
                .ToListAsync();
            ViewBag.TradSystems = await Context.TradSystems.Where(s => s.IsActive).OrderBy(s => s.SystemName).ToListAsync();
            ViewBag.Patterns = await Context.Patterns.Where(p => p.IsActive).OrderBy(p => p.PatternName).ToListAsync();
            ViewBag.Sessions = await Context.TradingSessions.Where(s => s.IsActive).OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToListAsync();
            ViewBag.AnalysisOutcomes = await Context.TradeAnalysisOutcomes.Where(o => o.IsActive).OrderBy(o => o.SortOrder).ThenBy(o => o.Name).ToListAsync();
            ViewBag.Emotions = await Context.TradeEmotions.Where(e => e.IsActive).OrderBy(e => e.Category).ThenBy(e => e.SortOrder).ThenBy(e => e.Name).ToListAsync();
            ViewBag.Directions = Enum.GetValues<TradeDirection>();
            ViewBag.Statuses = Enum.GetValues<TradeStatus>();
            ViewBag.Timeframes = Enum.GetValues<Timeframe>();
        }

        private async Task ApplySystemsAndPatterns(Trade trade, int[] systemIds, int[] patternIds, bool isEdit = false)
        {
            if (isEdit)
            {
                Context.TradeTradeSystems.RemoveRange(trade.TradeTradeSystems);
                Context.TradeTradePatterns.RemoveRange(trade.TradeTradePatterns);
                trade.TradeTradeSystems.Clear();
                trade.TradeTradePatterns.Clear();
            }

            if (systemIds?.Length > 0)
            {
                var systems = await Context.TradSystems.Where(s => systemIds.Contains(s.Id)).ToListAsync();
                trade.TradeTradeSystems = systems.Select(s => new TradeTradeSystem { TradSystemId = s.Id }).ToList();
            }

            if (patternIds?.Length > 0)
            {
                var patterns = await Context.Patterns.Where(p => patternIds.Contains(p.Id)).ToListAsync();
                trade.TradeTradePatterns = patterns.Select(p => new TradeTradePattern { PatternId = p.Id }).ToList();
            }
        }

        private async Task ApplyEmotions(Trade trade, int[] emotionIds, bool isEdit = false)
        {
            if (isEdit)
            {
                Context.TradeTradeEmotions.RemoveRange(trade.TradeTradeEmotions);
                trade.TradeTradeEmotions.Clear();
            }

            if (emotionIds?.Length > 0)
            {
                var emotions = await Context.TradeEmotions
                    .Where(e => emotionIds.Contains(e.Id) && e.IsActive)
                    .ToListAsync();
                trade.TradeTradeEmotions = emotions
                    .Select(e => new TradeTradeEmotion
                    {
                        TradeEmotionId = e.Id,
                        Category = e.Category
                    })
                    .ToList();
            }
        }

        private static async Task SaveAttachments(Trade trade, List<IFormFile>? files, bool isEdit = false)
        {
            if (files == null || files.Count == 0)
                return;

            var baseDirectory = Path.GetFullPath(BasePath);
            var tradeDate = trade.StartTime.ToString("yyyy-MM-dd");

            var safeOrderNumber = SanitizePathSegment(trade.OrderNumber);
            var relativeDirectory = Path.Combine(tradeDate, safeOrderNumber);
            var fullDirectory = Path.GetFullPath(
                Path.Combine(baseDirectory, relativeDirectory));

            // Prevent directory traversal.
            EnsurePathIsInsideBaseDirectory(baseDirectory, fullDirectory);

            Directory.CreateDirectory(fullDirectory);

            foreach (var file in files)
            {
                if (file == null || file.Length <= 0)
                    continue;

                // Keep the original name for display, but never use it as the physical name.
                var originalFileName = Path.GetFileName(file.FileName);

                if (string.IsNullOrWhiteSpace(originalFileName))
                    continue;

                var extension = Path.GetExtension(originalFileName);

                // Generate a unique physical filename.
                var storedFileName = $"{Guid.NewGuid():N}{extension}";

                var fullPath = Path.Combine(fullDirectory, storedFileName);
                var relativePath = Path.Combine(relativeDirectory, storedFileName);

                await using (var stream = new FileStream(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true))
                {
                    await file.CopyToAsync(stream);
                }

                trade.Attachments.Add(new TradeAttachment
                {
                    FileName = originalFileName,
                    FilePath = relativePath,
                    ContentType = file.ContentType,
                    SizeBytes = file.Length,
                    UploadedDate = DateTime.UtcNow
                });
            }
        }

        private static string SanitizePathSegment(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "NO-ORDER";

            var invalidChars = Path.GetInvalidFileNameChars()
                .Concat(['/', '\\'])
                .Distinct()
                .ToHashSet();

            var sanitized = new string(
                value.Trim()
                    .Select(c => invalidChars.Contains(c) ? '_' : c)
                    .ToArray());

            sanitized = sanitized.Trim('.', ' ');

            return string.IsNullOrWhiteSpace(sanitized)
                ? "NO-ORDER"
                : sanitized;
        }

        private static void EnsurePathIsInsideBaseDirectory(string baseDirectory, string targetPath)
        {
            var normalizedBase = Path.GetFullPath(baseDirectory)
                .TrimEnd(Path.DirectorySeparatorChar,
                         Path.AltDirectorySeparatorChar);

            var normalizedTarget = Path.GetFullPath(targetPath);

            var allowedPrefix = normalizedBase + Path.DirectorySeparatorChar;

            if (!normalizedTarget.StartsWith(
                    allowedPrefix,
                    OperatingSystem.IsWindows()
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal)
                && !string.Equals(
                    normalizedTarget,
                    normalizedBase,
                    OperatingSystem.IsWindows()
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal))
            {
                throw new InvalidOperationException("The attachment path is outside the configured storage directory.");
            }
        }

        private static void DeleteAttachmentFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var baseDirectory = Path.GetFullPath(BasePath);
            var fullPath = Path.GetFullPath(
                Path.Combine(baseDirectory, relativePath));

            // Do not allow deleting files outside BasePath.
            EnsurePathIsInsideBaseDirectory(baseDirectory, fullPath);

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            // Remove empty parent folders, without deleting BasePath itself.
            var baseFullPath = Path.GetFullPath(baseDirectory)
                .TrimEnd(Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);

            var directory = Path.GetDirectoryName(fullPath);

            while (!string.IsNullOrWhiteSpace(directory))
            {
                var currentDirectory = Path.GetFullPath(directory)
                    .TrimEnd(Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar);

                if (string.Equals(
                        currentDirectory,
                        baseFullPath,
                        OperatingSystem.IsWindows()
                            ? StringComparison.OrdinalIgnoreCase
                            : StringComparison.Ordinal))
                {
                    break;
                }

                EnsurePathIsInsideBaseDirectory(baseFullPath, currentDirectory);

                if (!Directory.Exists(currentDirectory) ||
                    Directory.EnumerateFileSystemEntries(currentDirectory).Any())
                {
                    break;
                }

                Directory.Delete(currentDirectory);
                directory = Path.GetDirectoryName(currentDirectory);
            }
        }

        #endregion

    }
}
