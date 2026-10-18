using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Helpers;
using TradingProject.Models.Enums;
using TradingProject.Security;

namespace TradingProject.Controllers
{
    [Authorize]
    public class DashboardController(ApplicationDbContext context) : BaseController
    {
        private ApplicationDbContext Context { get; } = context;

        public IActionResult Index()
        {
            if (!HasPermission(PermissionCodes.DashboardView))
                return Forbid();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetKpis(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var query = BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId);
            var closedTrades = query.Where(t => t.Status == TradeStatus.Closed);

            var totalTrades = await query.CountAsync();
            var closedCount = await closedTrades.CountAsync();

            var netPnl = await closedTrades.SumAsync(t => t.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0)));
            var grossProfit = await closedTrades.Where(t => (t.NetProfitLoss ?? t.RealizedPnL) > 0)
                .SumAsync(t => t.NetProfitLoss ?? t.RealizedPnL);
            var grossLoss = await closedTrades.Where(t => (t.NetProfitLoss ?? t.RealizedPnL) <= 0)
                .SumAsync(t => t.NetProfitLoss ?? t.RealizedPnL);

            var wins = await closedTrades.CountAsync(t =>
                (t.NetProfitLoss ?? t.RealizedPnL) > 0 ||
                ((t.NetProfitLoss ?? t.RealizedPnL) == null && t.IsPass));
            var losses = closedCount - wins;
            var winRate = closedCount > 0 ? Math.Round((double)wins / closedCount * 100, 1) : 0;

            var profitFactor = grossLoss.HasValue && grossLoss != 0
                ? Math.Round((double)(grossProfit ?? 0) / Math.Abs((double)grossLoss.Value), 2)
                : grossProfit > 0 ? double.PositiveInfinity : 0;

            var avgRr = await closedTrades.Where(t => t.RiskRewardRatio.HasValue)
                .AverageAsync(t => (decimal?)t.RiskRewardRatio);
            var avgDuration = await closedTrades.Where(t => t.DurationMinutes.HasValue)
                .AverageAsync(t => (double?)t.DurationMinutes);

            return Json(new
            {
                totalTrades,
                closedCount,
                wins,
                losses,
                netPnl = netPnl ?? 0,
                winRate,
                profitFactor = double.IsInfinity(profitFactor) ? 0 : profitFactor,
                avgRiskReward = avgRr.HasValue ? Math.Round(avgRr.Value, 2) : 0m,
                avgDurationMinutes = avgDuration.HasValue ? Math.Round(avgDuration.Value, 0) : 0d,
                avgDuration = TradeMetricsCalculator.FormatDuration(avgDuration.HasValue ? (int)Math.Round(avgDuration.Value) : null)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyPnl(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var query = BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId)
                .Where(t => t.Status == TradeStatus.Closed && (t.NetProfitLoss.HasValue || t.RealizedPnL.HasValue));

            var data = await query
                .GroupBy(t => new { t.StartTime.Year, t.StartTime.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    NetPnl = g.Sum(t => t.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0))),
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            return Json(data.Select(d => new
            {
                label = $"{d.Year.ToPersianYear():0000}-{d.Month.ToPersianMonth(d.Year):00}",
                netPnl = d.NetPnl,
                count = d.Count
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GetEquityCurve(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var query = BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId)
                .Where(t => t.Status == TradeStatus.Closed && (t.NetProfitLoss.HasValue || t.RealizedPnL.HasValue))
                .OrderBy(t => t.StartTime);

            var trades = await query
                .Select(t => new { t.StartTime, NetPnl = t.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0)) })
                .ToListAsync();

            decimal running = 0;
            var curve = trades.Select(t =>
            {
                running += t.NetPnl ?? 0;
                return new { date = t.StartTime.ToString("yyyy-MM-dd"), equity = running };
            }).ToList();

            return Json(curve);
        }

        [HttpGet]
        public async Task<IActionResult> GetDrawdown(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var query = BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId)
                .Where(t => t.Status == TradeStatus.Closed && (t.NetProfitLoss.HasValue || t.RealizedPnL.HasValue))
                .OrderBy(t => t.StartTime);

            var trades = await query
                .Select(t => new { t.StartTime, NetPnL = t.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0)) })
                .ToListAsync();

            decimal running = 0;
            decimal peak = 0;
            var curve = trades.Select(t =>
            {
                running += t.NetPnL ?? 0;
                if (running > peak) peak = running;
                var drawdown = running - peak;
                return new { date = t.StartTime.ToString("yyyy-MM-dd"), drawdown };
            }).ToList();

            return Json(curve);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopSystems(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId, int top = 5)
        {
            var tradeIds = await BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId)
                .Where(t => t.Status == TradeStatus.Closed)
                .Select(t => t.Id)
                .ToListAsync();

            var data = await Context.TradeTradeSystems
                .Where(j => tradeIds.Contains(j.TradeId))
                .GroupBy(j => j.TradSystemId)
                .Select(g => new
                {
                    SystemId = g.Key,
                    Count = g.Count(),
                    TradeIds = g.Select(j => j.TradeId).ToList()
                })
                .ToListAsync();

            var trades = await Context.Trades
                .Where(t => tradeIds.Contains(t.Id))
                .Select(t => new { t.Id, t.RealizedPnL, t.Fees, t.NetProfitLoss, t.IsPass })
                .ToListAsync();

            var systems = await Context.TradSystems.ToDictionaryAsync(s => s.Id, s => s.SystemName);

            var result = data
                .Select(d =>
                {
                    var ts = trades.Where(t => d.TradeIds.Contains(t.Id)).ToList();
                    return new
                    {
                        SystemName = systems.GetValueOrDefault(d.SystemId, "?"),
                        Count = d.Count,
                        Wins = ts.Count(t => (t.NetProfitLoss ?? t.RealizedPnL) > 0 || ((t.NetProfitLoss ?? t.RealizedPnL) == null && t.IsPass)),
                        NetPnl = ts.Sum(t => t.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0))) ?? 0
                    };
                })
                .OrderByDescending(x => x.NetPnl)
                .Take(top)
                .ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopPatterns(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId, int top = 5)
        {
            var tradeIds = await BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId)
                .Where(t => t.Status == TradeStatus.Closed)
                .Select(t => t.Id)
                .ToListAsync();

            var data = await Context.TradeTradePatterns
                .Where(j => tradeIds.Contains(j.TradeId))
                .GroupBy(j => j.PatternId)
                .Select(g => new
                {
                    PatternId = g.Key,
                    Count = g.Count(),
                    TradeIds = g.Select(j => j.TradeId).ToList()
                })
                .ToListAsync();

            var trades = await Context.Trades
                .Where(t => tradeIds.Contains(t.Id))
                .Select(t => new { t.Id, t.RealizedPnL, t.Fees, t.NetProfitLoss, t.IsPass })
                .ToListAsync();

            var patterns = await Context.Patterns.ToDictionaryAsync(p => p.Id, p => p.PatternName);

            var result = data
                .Select(d =>
                {
                    var ts = trades.Where(t => d.TradeIds.Contains(t.Id)).ToList();
                    return new
                    {
                        PatternName = patterns.GetValueOrDefault(d.PatternId, "?"),
                        Count = d.Count,
                        Wins = ts.Count(t => (t.NetProfitLoss ?? t.RealizedPnL) > 0 || ((t.NetProfitLoss ?? t.RealizedPnL) == null && t.IsPass)),
                        NetPnl = ts.Sum(t => t.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0))) ?? 0
                    };
                })
                .OrderByDescending(x => x.NetPnl)
                .Take(top)
                .ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetWinLoss(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var filtered = BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId);
            var closed = filtered.Where(t => t.Status == TradeStatus.Closed);

            var wins = await closed.CountAsync(t =>
                (t.NetProfitLoss ?? t.RealizedPnL) > 0 ||
                ((t.NetProfitLoss ?? t.RealizedPnL) == null && t.IsPass));
            var total = await closed.CountAsync();
            var losses = total - wins;
            var open = await filtered.CountAsync(t => t.Status == TradeStatus.Open);

            return Json(new { wins, losses, open, total });
        }

        [HttpGet]
        public async Task<IActionResult> GetSessionPerformance(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var trades = await BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId)
                .Where(t => t.Status == TradeStatus.Closed)
                .Select(t => new
                {
                    t.TradingSessionId,
                    SessionName = t.TradingSession != null ? t.TradingSession.Name : null,
                    SessionNameFa = t.TradingSession != null ? t.TradingSession.NameFa : null,
                    t.NetProfitLoss,
                    t.RealizedPnL,
                    t.Fees,
                    t.IsPass
                })
                .ToListAsync();

            var result = trades
                .GroupBy(t => t.TradingSessionId)
                .Select(g =>
                {
                    var wins = g.Count(t => (t.NetProfitLoss ?? t.RealizedPnL) > 0 || ((t.NetProfitLoss ?? t.RealizedPnL) == null && t.IsPass));
                    var first = g.First();
                    return new
                    {
                        sessionName = first.TradingSessionId == null
                            ? Resources.Resources.Unassigned
                            : LocaleHelper.Pick(first.SessionName ?? Resources.Resources.Unassigned, first.SessionNameFa),
                        count = g.Count(),
                        wins,
                        netPnl = g.Sum(t => t.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0))) ?? 0
                    };
                })
                .OrderByDescending(x => x.netPnl)
                .ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmotionPerformance(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var tradeIds = await BuildFilteredQuery(fromDate, toDate, symbolId, brokerId, accountId, systemId, patternId, sessionId, status, isDemo, direction, userId)
                .Where(t => t.Status == TradeStatus.Closed)
                .Select(t => t.Id)
                .ToListAsync();

            var links = await Context.TradeTradeEmotions
                .Where(j => tradeIds.Contains(j.TradeId))
                .Select(j => new
                {
                    j.TradeId,
                    j.Category,
                    j.TradeEmotionId,
                    Name = j.TradeEmotion != null ? j.TradeEmotion.Name : "?",
                    NameFa = j.TradeEmotion != null ? j.TradeEmotion.NameFa : null
                })
                .ToListAsync();

            var trades = await Context.Trades
                .Where(t => tradeIds.Contains(t.Id))
                .Select(t => new { t.Id, t.RealizedPnL, t.Fees, t.NetProfitLoss, t.IsPass })
                .ToListAsync();

            var tradeMap = trades.ToDictionary(t => t.Id);
            var result = links
                .GroupBy(l => new { l.TradeEmotionId, l.Category, l.Name, l.NameFa })
                .Select(g =>
                {
                    var ts = g.Select(x => tradeMap.GetValueOrDefault(x.TradeId)).Where(t => t != null).ToList();
                    var wins = ts.Count(t => (t!.NetProfitLoss ?? t.RealizedPnL) > 0 || ((t.NetProfitLoss ?? t.RealizedPnL) == null && t.IsPass));
                    return new
                    {
                        category = Resources.Resources.ResourceManager.GetString("Emotion_" + g.Key.Category) ?? g.Key.Category.ToString(),
                        emotion = LocaleHelper.Pick(g.Key.Name, g.Key.NameFa),
                        count = ts.Count,
                        wins,
                        losses = ts.Count - wins,
                        netPnl = ts.Sum(t => t!.NetProfitLoss ?? (t.RealizedPnL - (t.Fees ?? 0))) ?? 0
                    };
                })
                .OrderByDescending(x => x.count)
                .Take(12)
                .ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions()
        {
            var accountQuery = Context.TradingAccounts.Where(a => a.IsActive);
            if (!HasPermission(PermissionCodes.UsersViewOthersTrades))
                accountQuery = accountQuery.Where(a => a.UserId == CurrentUserId);

            return Json(new
            {
                symbols = await Context.TradingSymbols.Where(s => s.IsActive).Select(s => new { s.Id, s.Symbol, s.Name, s.NameFa, Instrument = s.InstrumentType.ToString() }).ToListAsync(),
                brokers = await Context.Brokers.Where(b => b.IsActive).Select(b => new { b.Id, b.Name }).ToListAsync(),
                accounts = await accountQuery.Select(a => new { a.Id, a.Name }).ToListAsync(),
                systems = await Context.TradSystems.Where(s => s.IsActive).Select(s => new { s.Id, s.SystemName }).ToListAsync(),
                patterns = await Context.Patterns.Where(p => p.IsActive).Select(p => new { p.Id, p.PatternName }).ToListAsync(),
                sessions = await Context.TradingSessions.Where(s => s.IsActive)
                    .OrderBy(s => s.SortOrder)
                    .Select(s => new { s.Id, s.Name, s.NameFa })
                    .ToListAsync(),
                users = HasPermission(PermissionCodes.UsersViewOthersTrades)
                    ? await Context.Users.Where(u => u.IsActive).Select(u => new { u.Id, u.Username, u.DisplayName }).ToListAsync()
                    : null
            });
        }

        private IQueryable<Models.Trade> BuildFilteredQuery(
            DateTime? fromDate, DateTime? toDate, int? symbolId, int? brokerId,
            int? accountId, int? systemId, int? patternId, int? sessionId, TradeStatus? status,
            bool? isDemo, TradeDirection? direction, int? userId)
        {
            var query = Context.Trades.AsQueryable();

            if (userId.HasValue && HasPermission(PermissionCodes.UsersViewOthersTrades))
            {
                query = query.Where(t => t.UserId == userId.Value);
            }
            else if (!HasPermission(PermissionCodes.UsersViewOthersTrades))
            {
                query = query.Where(t => t.UserId == CurrentUserId);
            }

            if (fromDate.HasValue) query = query.Where(t => t.StartTime >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(t => t.StartTime <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            if (symbolId.HasValue) query = query.Where(t => t.SymbolId == symbolId.Value);
            if (brokerId.HasValue) query = query.Where(t => t.BrokerId == brokerId.Value);
            if (accountId.HasValue) query = query.Where(t => t.TradingAccountId == accountId.Value);
            if (systemId.HasValue) query = query.Where(t => t.TradeTradeSystems.Any(j => j.TradSystemId == systemId.Value));
            if (patternId.HasValue) query = query.Where(t => t.TradeTradePatterns.Any(j => j.PatternId == patternId.Value));
            if (sessionId.HasValue) query = query.Where(t => t.TradingSessionId == sessionId.Value);
            if (status.HasValue) query = query.Where(t => t.Status == status.Value);
            if (isDemo.HasValue) query = query.Where(t => t.IsDemoAccount == isDemo.Value);
            if (direction.HasValue) query = query.Where(t => t.Direction == direction.Value);

            return query;
        }
    }
}
