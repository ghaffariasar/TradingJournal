using TradingProject.Models;

namespace TradingProject.Services
{
    public interface IPermissionService
    {
        Task LoadCurrentAsync(HttpContext httpContext);
        bool CurrentHas(string permissionCode);
        Task<HashSet<string>> GetEffectiveCodesAsync(int userId);
        Task<bool> HasAsync(int userId, string permissionCode);
        Task<int> CountActiveAdministratorsAsync(int? excludingUserId = null);
        Task ApplyUserOverridesAsync(int userId, IReadOnlyCollection<int> grantedPermissionIds);
        Task SetRolePermissionsAsync(int roleId, IReadOnlyCollection<int> permissionIds);
        bool IsCurrentAdministrator { get; }
    }
}
