using System.ComponentModel.DataAnnotations;

namespace TradingProject.Models
{
    public class TradeAttachment
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "TradeId")]
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

        [Display(
            Name = nameof(Resources.Resources.FileName),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            256,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string FileName { get; set; } = string.Empty;

        [Display(
            Name = nameof(Resources.Resources.FilePath),
            ResourceType = typeof(Resources.Resources))]
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.Required))]
        [StringLength(
            512,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = nameof(Resources.Resources.StringLength))]
        public string FilePath { get; set; } = string.Empty;

        [Display(Name = "ContentType")]
        [StringLength(128)]
        public string? ContentType { get; set; }

        [Display(Name = "SizeBytes")]
        public long? SizeBytes { get; set; }

        [Display(Name = "UploadedDate")]
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
