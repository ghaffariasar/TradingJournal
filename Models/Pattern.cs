using System.ComponentModel.DataAnnotations;

namespace TradingProject.Models
{
    public class Pattern
    {
        [Key]
        public int Id { get; set; }

        [Display(
            Name = nameof(Resources.Resources.PatternName),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            128,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string PatternName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(512)]
        public string? Description { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<TradeTradePattern> TradeTradePatterns { get; set; } = [];
    }
}
