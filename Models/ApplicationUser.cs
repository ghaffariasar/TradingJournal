using System.ComponentModel.DataAnnotations;
using TradingProject.Models.Enums;

namespace TradingProject.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }

      
        [Display(
            Name = nameof(Resources.Resources.Username),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            64,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string Username { get; set; } = string.Empty;

        [Display(
            Name = nameof(Resources.Resources.Password),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        public string PasswordHash { get; set; } = string.Empty;

        [Display(Name = "DisplayName")]
        [StringLength(
            128,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string? DisplayName { get; set; }

        [Display(Name = "Email")]
        [StringLength(
            128,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string? Email { get; set; }

        [Display(Name = "Role")]
        public UserRole Role { get; set; } = UserRole.Trader;

        [Display(Name = "Role")]
        public int? RoleId { get; set; }

        public AppRole? AppRole { get; set; }

        public List<UserPermissionOverride> PermissionOverrides { get; set; } = [];

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        public List<Trade> Trades { get; set; } = [];
        public List<TradingAccount> TradingAccounts { get; set; } = [];
    }
}
