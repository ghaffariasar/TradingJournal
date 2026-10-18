using System.ComponentModel.DataAnnotations;
using TradingProject.Helpers;

namespace TradingProject.Models
{
    public class AppRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string Name { get; set; } = string.Empty;

        [StringLength(128)]
        public string? NameFa { get; set; }

        [StringLength(512)]
        public string? Description { get; set; }

        public bool IsSystem { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<RolePermission> RolePermissions { get; set; } = [];
        public List<ApplicationUser> Users { get; set; } = [];

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string LocalizedName => LocaleHelper.Pick(Name, NameFa);
    }
}
