namespace TradingProject.Models
{
    public class TradeTradeSystem
    {
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

        public int TradSystemId { get; set; }
        public TradSystem? TradSystem { get; set; }
    }
}
