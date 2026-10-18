using TradingProject.Models;
using TradingProject.Models.Enums;

namespace TradingProject.Helpers
{
    public static class TradeMetricsCalculator
    {
        public static void Apply(Trade trade)
        {
            trade.NetProfitLoss = CalculateNetProfitLoss(trade);
            trade.DurationMinutes = CalculateDurationMinutes(trade);
            trade.RiskRewardRatio = CalculateRiskRewardRatio(trade);
        }

        public static decimal? CalculateNetProfitLoss(Trade trade)
        {
            if (!trade.RealizedPnL.HasValue)
                return null;

            return decimal.Round(trade.RealizedPnL.Value - (trade.Fees ?? 0m), 2);
        }

        public static int? CalculateDurationMinutes(Trade trade)
        {
            if (!trade.EndTime.HasValue)
                return null;

            var duration = trade.EndTime.Value - trade.StartTime;
            if (duration < TimeSpan.Zero)
                return null;

            return (int)Math.Round(duration.TotalMinutes, MidpointRounding.AwayFromZero);
        }

        public static decimal? CalculateRiskRewardRatio(Trade trade)
        {
            if (trade.EntryPrice <= 0 || !trade.StopLoss.HasValue)
                return null;

            var risk = trade.Direction == TradeDirection.Long
                ? trade.EntryPrice - trade.StopLoss.Value
                : trade.StopLoss.Value - trade.EntryPrice;

            if (risk <= 0)
                return null;

            decimal? rewardDistance = null;
            if (trade.TakeProfit.HasValue)
            {
                rewardDistance = trade.Direction == TradeDirection.Long
                    ? trade.TakeProfit.Value - trade.EntryPrice
                    : trade.EntryPrice - trade.TakeProfit.Value;
            }
            else if (trade.ExitPrice.HasValue)
            {
                rewardDistance = trade.Direction == TradeDirection.Long
                    ? trade.ExitPrice.Value - trade.EntryPrice
                    : trade.EntryPrice - trade.ExitPrice.Value;
            }

            if (!rewardDistance.HasValue || rewardDistance.Value <= 0)
                return null;

            return decimal.Round(rewardDistance.Value / risk, 4);
        }

        public static string FormatDuration(int? minutes)
        {
            if (!minutes.HasValue)
                return "—";

            var ts = TimeSpan.FromMinutes(Math.Abs(minutes.Value));
            if (ts.TotalDays >= 1)
                return $"{(int)ts.TotalDays}d {ts.Hours}h {ts.Minutes}m";
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}h {ts.Minutes}m";
            return $"{ts.Minutes}m";
        }

        public static TradingPerformanceMetrics CalculatePerformance(IEnumerable<Trade> trades)
        {
            var tradeList = trades.ToList();
            var metrics = new TradingPerformanceMetrics
            {
                TotalTrades = tradeList.Count
            };

            if (metrics.TotalTrades == 0)
                return metrics;

            var closedTrades = tradeList.Where(t => t.Status == TradeStatus.Closed).ToList();
            metrics.WinningTrades = closedTrades.Count(t => (t.NetProfitLoss ?? 0) > 0);
            metrics.WinRate = closedTrades.Count > 0 ? (decimal)metrics.WinningTrades / closedTrades.Count * 100 : 0;
            metrics.NetProfitLoss = closedTrades.Sum(t => t.NetProfitLoss ?? 0);
            metrics.AverageTradeResult = closedTrades.Count > 0 ? metrics.NetProfitLoss / closedTrades.Count : 0;

            var grossProfit = closedTrades.Where(t => (t.NetProfitLoss ?? 0) > 0).Sum(t => t.NetProfitLoss ?? 0);
            var grossLoss = Math.Abs(closedTrades.Where(t => (t.NetProfitLoss ?? 0) < 0).Sum(t => t.NetProfitLoss ?? 0));

            metrics.ProfitFactor = grossLoss == 0 ? (grossProfit > 0 ? 99.99m : 0) : grossProfit / grossLoss;

            return metrics;
        }
    }

    public class TradingPerformanceMetrics
    {
        public int TotalTrades { get; set; }
        public int WinningTrades { get; set; }
        public decimal WinRate { get; set; }
        public decimal NetProfitLoss { get; set; }
        public decimal AverageTradeResult { get; set; }
        public decimal ProfitFactor { get; set; }
    }
}
