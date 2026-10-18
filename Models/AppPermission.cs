using System.ComponentModel.DataAnnotations;
using TradingProject.Helpers;

namespace TradingProject.Models
{
    public class AppPermission
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

        [Required]
        [StringLength(64)]
        public string Module { get; set; } = string.Empty;

        [StringLength(256)]
        public string? Description { get; set; }

        [StringLength(256)]
        public string? DescriptionFa { get; set; }

        public int SortOrder { get; set; }

        public List<RolePermission> RolePermissions { get; set; } = [];
        public List<UserPermissionOverride> UserOverrides { get; set; } = [];

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string LocalizedName => LocaleHelper.Pick(Name, NameFa);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string LocalizedDescription => LocaleHelper.Pick(Description ?? string.Empty, DescriptionFa ?? string.Empty);
    }
}
