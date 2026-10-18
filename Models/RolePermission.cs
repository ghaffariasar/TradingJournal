namespace TradingProject.Models
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public AppRole? Role { get; set; }

        public int PermissionId { get; set; }
        public AppPermission? Permission { get; set; }
    }
}
