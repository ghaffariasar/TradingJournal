using System.ComponentModel.DataAnnotations;
using TradingProject.Models.Enums;

namespace TradingProject.Models
{
    public class TradingSymbol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Symbol")]
        [StringLength(32)]
        public string Symbol { get; set; } = string.Empty;

        [Display(Name = "Name")]
        [StringLength(128)]
        public string? Name { get; set; }

        [Display(Name = "NameFa")]
        [StringLength(255)]
        public string? NameFa { get; set; }

        [Display(Name = "InstrumentType")]
        public InstrumentType InstrumentType { get; set; } = InstrumentType.Forex;

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<Trade> Trades { get; set; } = [];
    }
}
