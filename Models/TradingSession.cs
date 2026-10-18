using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingProject.Helpers;

namespace TradingProject.Models
{
    public class TradingSession
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
        public string Name { get; set; } = string.Empty;

        [Display(
            Name = nameof(Resources.Resources.NameFa),
            ResourceType = typeof(Resources.Resources))]
        [StringLength(
            128,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string? NameFa { get; set; }

        [Display(
            Name = nameof(Resources.Resources.Description),
            ResourceType = typeof(Resources.Resources))]
        [StringLength(
            512,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string? Description { get; set; }

        [Display(
            Name = nameof(Resources.Resources.StartTimeOfDay),
            ResourceType = typeof(Resources.Resources))]
        public TimeSpan? StartTimeOfDay { get; set; }

        [Display(
            Name = nameof(Resources.Resources.EndTimeOfDay),
            ResourceType = typeof(Resources.Resources))]
        public TimeSpan? EndTimeOfDay { get; set; }

        [Display(
            Name = nameof(Resources.Resources.IsActive),
            ResourceType = typeof(Resources.Resources))]
        public bool IsActive { get; set; } = true;

        [Display(
            Name = nameof(Resources.Resources.SortOrder),
            ResourceType = typeof(Resources.Resources))]
        public int SortOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<Trade> Trades { get; set; } = [];

        [NotMapped]
        public string LocalizedName => LocaleHelper.Pick(Name, NameFa);
    }
}
