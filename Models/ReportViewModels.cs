using TradingProject.Models;
using TradingProject.Helpers;

namespace TradingProject.Models
{
    public class UserTradeReportViewModel
    {
        public ApplicationUser User { get; set; } = new();
        public TradingPerformanceMetrics Metrics { get; set; } = new();
        public List<Trade> RecentTrades { get; set; } = [];
        
        // Filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? SystemId { get; set; }
        public int? PatternId { get; set; }
        public int? SessionId { get; set; }
    }
}
