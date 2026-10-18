namespace TradingProject.Models
{
    public class UserPermissionOverride
    {
        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int PermissionId { get; set; }
        public AppPermission? Permission { get; set; }

        public bool IsGranted { get; set; }
    }
}
