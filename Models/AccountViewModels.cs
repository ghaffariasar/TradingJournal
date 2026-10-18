using System.ComponentModel.DataAnnotations;

namespace TradingProject.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Resources.Resources))]
        [StringLength(64, MinimumLength = 3, ErrorMessageResourceName = "UsernameLength", ErrorMessageResourceType = typeof(Resources.Resources))]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessageResourceName = "UsernameRegex", ErrorMessageResourceType = typeof(Resources.Resources))]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Resources.Resources))]
        [StringLength(100, MinimumLength = 8, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(Resources.Resources))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessageResourceName = "PasswordRegex", ErrorMessageResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceName = "PasswordsDoNotMatch", ErrorMessageResourceType = typeof(Resources.Resources))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [StringLength(128)]
        public string? DisplayName { get; set; }

        [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(Resources.Resources))]
        [StringLength(128)]
        public string? Email { get; set; }
    }
}
