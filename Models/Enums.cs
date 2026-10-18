namespace TradingProject.Models.Enums
{
    public enum TradeDirection
    {
        Long = 1,
        Short = 2
    }

    public enum TradeStatus
    {
        Open = 1,
        Closed = 2,
        Cancelled = 3
    }

    public enum Timeframe
    {
        M1 = 1,
        M5 = 2,
        M15 = 3,
        M30 = 4,
        H1 = 5,
        H4 = 6,
        D1 = 7,
        W1 = 8,
        MN = 9
    }

    public enum UserRole
    {
        Admin = 1,
        Trader = 2,
        Operator = 3
    }

    public enum InstrumentType
    {
        Forex = 1,
        Crypto = 2,
        Index = 3,
        Commodity = 4,
        Stock = 5,
        Other = 6
    }

    public enum EmotionCategory
    {
        AtEntry = 1,
        DuringTrade = 2,
        AfterClose = 3
    }
}
