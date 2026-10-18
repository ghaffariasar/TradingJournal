using TradingProject.Models.Enums;

namespace TradingProject.Models
{
    public class TradeTradeEmotion
    {
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

        public int TradeEmotionId { get; set; }
        public TradeEmotion? TradeEmotion { get; set; }

        public EmotionCategory Category { get; set; }
    }
}
