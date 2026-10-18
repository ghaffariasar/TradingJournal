namespace TradingProject.Models
{
    public class TradeTradePattern
    {
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

        public int PatternId { get; set; }
        public Pattern? Pattern { get; set; }
    }
}
