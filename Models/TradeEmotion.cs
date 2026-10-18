using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingProject.Helpers;
using TradingProject.Models.Enums;

namespace TradingProject.Models
{
    public class TradeEmotion
    {
        [Key]
        public int Id { get; set; }

        [Display(
            Name = nameof(Resources.Resources.EmotionCategory),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        public EmotionCategory Category { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Name),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            256,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string Name { get; set; } = string.Empty;

        [Display(
            Name = nameof(Resources.Resources.NameFa),
            ResourceType = typeof(Resources.Resources))]
        [StringLength(
            256,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string? NameFa { get; set; }

        [Display(
            Name = nameof(Resources.Resources.IsActive),
            ResourceType = typeof(Resources.Resources))]
        public bool IsActive { get; set; } = true;

        [Display(
            Name = nameof(Resources.Resources.SortOrder),
            ResourceType = typeof(Resources.Resources))]
        public int SortOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<TradeTradeEmotion> TradeTradeEmotions { get; set; } = [];

        [NotMapped]
        public string LocalizedName => LocaleHelper.Pick(Name, NameFa);
    }
}
