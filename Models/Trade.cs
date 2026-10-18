using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TradingProject.Models.Enums;

namespace TradingProject.Models
{
    public class Trade : IValidatableObject
    {

        [Key]
        public int Id { get; set; }

        [Display(
            Name = nameof(Resources.Resources.OrderNumber),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            50,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string OrderNumber { get; set; } = string.Empty;

        [Display(
            Name = nameof(Resources.Resources.Symbol),
            ResourceType = typeof(Resources.Resources))]
        public int? SymbolId { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Symbol),
            ResourceType = typeof(Resources.Resources))]
        public TradingSymbol? Symbol { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [Display(
            Name = nameof(Resources.Resources.TradeSymbol),
            ResourceType = typeof(Resources.Resources))]
        [StringLength(
            64,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string TradeSymbol { get; set; } = string.Empty;

        [Display(
            Name = nameof(Resources.Resources.Broker),
            ResourceType = typeof(Resources.Resources))]
        public int? BrokerId { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Broker),
            ResourceType = typeof(Resources.Resources))]
        public Broker? Broker { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TradingAccount),
            ResourceType = typeof(Resources.Resources))]
        public int? TradingAccountId { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TradingAccount),
            ResourceType = typeof(Resources.Resources))]
        public TradingAccount? TradingAccount { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TradingSession),
            ResourceType = typeof(Resources.Resources))]
        public int? TradingSessionId { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TradingSession),
            ResourceType = typeof(Resources.Resources))]
        public TradingSession? TradingSession { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Direction),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        public TradeDirection Direction { get; set; } = TradeDirection.Long;

        [Display(
            Name = nameof(Resources.Resources.Status),
            ResourceType = typeof(Resources.Resources))]
        public TradeStatus Status { get; set; } = TradeStatus.Closed;

        [Display(
            Name = nameof(Resources.Resources.Timeframe),
            ResourceType = typeof(Resources.Resources))]
        public Timeframe? Timeframe { get; set; }

        [Display(
            Name = nameof(Resources.Resources.StartTime),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        public DateTime StartTime { get; set; }

        [Display(
            Name = nameof(Resources.Resources.EndTime),
            ResourceType = typeof(Resources.Resources))]
        public DateTime? EndTime { get; set; }

        [Display(
            Name = nameof(Resources.Resources.AccountBalance),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 2)]
        public decimal? AccountBalance { get; set; }

        [Display(
            Name = nameof(Resources.Resources.LotSize),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [Range(
            0.00001,
            double.MaxValue,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.RangeMin))]
        [Precision(18, 2)]
        public decimal LotSize { get; set; }

        [Display(
            Name = nameof(Resources.Resources.EntryPrice),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [Range(
            0.00001,
            double.MaxValue,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.RangeMin))]
        [Precision(18, 5)]
        public decimal EntryPrice { get; set; }

        [Display(
            Name = nameof(Resources.Resources.ExitPrice),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 5)]
        public decimal? ExitPrice { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TakeProfit),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 5)]
        public decimal? TakeProfit  { get; set; }

        [Display(
            Name = nameof(Resources.Resources.StopLoss),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 5)]
        public decimal? StopLoss { get; set; }

        [Display(
            Name = nameof(Resources.Resources.RealizedPnL),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 2)]
        public decimal? RealizedPnL { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Fees),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 2)]
        public decimal? Fees { get; set; }

        [Display(
            Name = nameof(Resources.Resources.RiskAmount),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 2)]
        public decimal? RiskAmount { get; set; }

     
        [Display(
            Name = nameof(Resources.Resources.ExitReason),
            ResourceType = typeof(Resources.Resources))]
        [StringLength(
            256,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string? ExitReason { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Notes),
            ResourceType = typeof(Resources.Resources))]
        public string? Notes { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TradSystems),
            ResourceType = typeof(Resources.Resources))]
        public List<TradeTradeSystem> TradeTradeSystems { get; set; } = [];

        [Display(
            Name = nameof(Resources.Resources.Patterns),
            ResourceType = typeof(Resources.Resources))]
        public List<TradeTradePattern> TradeTradePatterns { get; set; } = [];

        [Display(
            Name = nameof(Resources.Resources.TradeAnalysisOutcome),
            ResourceType = typeof(Resources.Resources))]
        public int? TradeAnalysisOutcomeId { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TradeAnalysisOutcome),
            ResourceType = typeof(Resources.Resources))]
        public TradeAnalysisOutcome? TradeAnalysisOutcome { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Emotions),
            ResourceType = typeof(Resources.Resources))]
        public List<TradeTradeEmotion> TradeTradeEmotions { get; set; } = [];

        [Display(
            Name = nameof(Resources.Resources.TrendConfirmation1M),
            ResourceType = typeof(Resources.Resources))]
        public bool TrendConfirmation1M { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TrendConfirmation5M),
            ResourceType = typeof(Resources.Resources))]
        public bool TrendConfirmation5M { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TrendConfirmation15M),
            ResourceType = typeof(Resources.Resources))]
        public bool TrendConfirmation15M { get; set; }

        [Display(
            Name = nameof(Resources.Resources.DivergenceHD),
            ResourceType = typeof(Resources.Resources))]
        public bool DivergenceHD { get; set; }

        [Display(
            Name = nameof(Resources.Resources.DivergenceRD),
            ResourceType = typeof(Resources.Resources))]
        public bool DivergenceRD { get; set; }

        [Display(
            Name = nameof(Resources.Resources.EndOfCorrection),
            ResourceType = typeof(Resources.Resources))]
        public bool EOC { get; set; }

        [Display(
            Name = nameof(Resources.Resources.EndOfWay),
            ResourceType = typeof(Resources.Resources))]
        public bool EOW { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Attachments),
            ResourceType = typeof(Resources.Resources))]
        public List<TradeAttachment> Attachments { get; set; } = [];

        [Display(
            Name = nameof(Resources.Resources.IsPass),
            ResourceType = typeof(Resources.Resources))]
        public bool IsPass { get; set; }

        [Display(
            Name = nameof(Resources.Resources.IsDemoAccount),
            ResourceType = typeof(Resources.Resources))]
        public bool IsDemoAccount { get; set; }

        [Display(
            Name = nameof(Resources.Resources.RiskRewardRatio),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 4)]
        public decimal? RiskRewardRatio { get; set; }

        [Display(
            Name = nameof(Resources.Resources.TradeDuration),
            ResourceType = typeof(Resources.Resources))]
        public int? DurationMinutes { get; set; }

        [Display(
            Name = nameof(Resources.Resources.NetPnL),
            ResourceType = typeof(Resources.Resources))]
        [Precision(18, 2)]
        public decimal? NetProfitLoss { get; set; }

        public int UserId { get; set; }

      
        public ApplicationUser? User { get; set; }

      
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        [Display(
            Name = nameof(Resources.Resources.NetPnL),
            ResourceType = typeof(Resources.Resources))]
        public decimal? NetPnL =>
            NetProfitLoss ?? (RealizedPnL.HasValue ? RealizedPnL - (Fees ?? 0) : null);

        [NotMapped]
        [Display(
            Name = nameof(Resources.Resources.Wins),
            ResourceType = typeof(Resources.Resources))]
        public bool IsWin =>
            (NetProfitLoss ?? RealizedPnL) switch
            {
                > 0 => true,
                < 0 => false,
                _ => IsPass
            };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime.HasValue && EndTime.Value < StartTime)
            {
                yield return new ValidationResult(
                    Resources.Resources.InvalidDateRange,
                    [nameof(EndTime)]);
            }

            if (Status == TradeStatus.Closed)
            {
                if (!EndTime.HasValue)
                {
                    yield return new ValidationResult(
                        Resources.Resources.ClosedTradeRequiresExit,
                        [nameof(EndTime)]);
                }

                if (!ExitPrice.HasValue || ExitPrice.Value <= 0)
                {
                    yield return new ValidationResult(
                        Resources.Resources.ClosedTradeRequiresExitPrice,
                        [nameof(ExitPrice)]);
                }
            }

            if (StopLoss.HasValue && StopLoss.Value <= 0)
            {
                yield return new ValidationResult(
                    Resources.Resources.RangeMin,
                    [nameof(StopLoss)]);
            }

            if (TakeProfit.HasValue && TakeProfit.Value <= 0)
            {
                yield return new ValidationResult(
                    Resources.Resources.RangeMin,
                    [nameof(TakeProfit)]);
            }
        }
    }
}
