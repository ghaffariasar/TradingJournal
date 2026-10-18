using System.ComponentModel.DataAnnotations;

namespace TradingProject.Models
{
    public class TradSystem
    {
        [Key]
        public int Id { get; set; }


        [Display(
            Name = nameof(Resources.Resources.Name),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            128,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string SystemName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(512)]
        public string? Description { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<TradeTradeSystem> TradeTradeSystems { get; set; } = [];
    }
}
