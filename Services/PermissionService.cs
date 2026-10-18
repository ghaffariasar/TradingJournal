using Microsoft.EntityFrameworkCore;
using TradingProject.Data;
using TradingProject.Models;
using TradingProject.Security;

namespace TradingProject.Services
{
    public class PermissionService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : IPermissionService
    {
        private const string ItemKey = "tj.permissions";
        private const int AdminRoleId = 1;

        public bool IsCurrentAdministrator => CurrentHas(PermissionCodes.UsersManageAccess);

        private HttpContext? HttpContext => httpContextAccessor.HttpContext;

        public async Task LoadCurrentAsync(HttpContext httpContext)
        {
            if (httpContext.Items.ContainsKey(ItemKey))
                return;

            var claim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId) || userId <= 0)
            {
                httpContext.Items[ItemKey] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                return;
            }

            httpContext.Items[ItemKey] = await GetEffectiveCodesAsync(userId);
        }

        public bool CurrentHas(string permissionCode)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
                return false;

            if (HttpContext == null)
                return false;

            if (HttpContext.Items.TryGetValue(ItemKey, out var val) && val is HashSet<string> codes)
            {
                return codes.Contains(permissionCode);
            }

            return false;
        }

        public async Task<HashSet<string>> GetEffectiveCodesAsync(int userId)
        {
            var user = await context.Users
                .AsNoTracking()
                .Include(u => u.AppRole)
                    .ThenInclude(r => r!.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Include(u => u.PermissionOverrides)
                    .ThenInclude(uo => uo.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return [];

            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1. Permissions from Role
            if (user.AppRole != null)
            {
                foreach (var rp in user.AppRole.RolePermissions)
                {
                    if (rp.Permission != null)
                        codes.Add(rp.Permission.Code);
                }
            }

            // 2. Overrides
            foreach (var uo in user.PermissionOverrides)
            {
                if (uo.Permission == null) 
                    continue;

                if (uo.IsGranted)
                    codes.Add(uo.Permission.Code);
                else
                    codes.Remove(uo.Permission.Code);
            }

            return codes;
        }

        public async Task<bool> HasAsync(int userId, string permissionCode)
        {
            var codes = await GetEffectiveCodesAsync(userId);
            return codes.Contains(permissionCode);
        }

        public async Task<int> CountActiveAdministratorsAsync(int? excludingUserId = null)
        {
            var query = context.Users.Where(u => u.IsActive && u.RoleId == AdminRoleId);
            if (excludingUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludingUserId.Value);
            }
            return await query.CountAsync();
        }

        public async Task ApplyUserOverridesAsync(int userId, IReadOnlyCollection<int> grantedPermissionIds)
        {
            // Remove existing overrides
            var existing = await context.UserPermissionOverrides.Where(uo => uo.UserId == userId).ToListAsync();
            context.UserPermissionOverrides.RemoveRange(existing);

            // Add new overrides
            foreach (var pid in grantedPermissionIds)
            {
                context.UserPermissionOverrides.Add(new UserPermissionOverride
                {
                    UserId = userId,
                    PermissionId = pid,
                    IsGranted = true
                });
            }

            await context.SaveChangesAsync();
        }

        public async Task SetRolePermissionsAsync(int roleId, IReadOnlyCollection<int> permissionIds)
        {
            var existing = await context.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
            context.RolePermissions.RemoveRange(existing);

            foreach (var pid in permissionIds)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = pid
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
