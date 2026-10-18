using System.ComponentModel.DataAnnotations;

namespace TradingProject.Models
{
    public class Broker
    {
        [Key]
        public int Id { get; set; }


        [Display(
            Name = nameof(Resources.Resources.Broker),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            128,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Website")]
        [StringLength(
            256,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string? Website { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<Trade> Trades { get; set; } = [];
        public List<TradingAccount> TradingAccounts { get; set; } = [];
    }
}
