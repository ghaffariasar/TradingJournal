using System.ComponentModel.DataAnnotations;

namespace TradingProject.Models
{
    public class TradingAccount
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

        [Display(Name = "AccountNumber")]
        [StringLength(64)]
        public string? AccountNumber { get; set; }

        [Display(Name = "BrokerId")]
        public int? BrokerId { get; set; }
        public Broker? Broker { get; set; }

        [Display(Name = "IsDemo")]
        public bool IsDemo { get; set; }

        [Display(Name = "Currency")]
        [StringLength(8)]
        public string? Currency { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public List<Trade> Trades { get; set; } = [];
    }
}
