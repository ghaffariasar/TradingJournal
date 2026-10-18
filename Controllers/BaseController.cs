using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TradingProject.Models.Enums;
using TradingProject.Services;

namespace TradingProject.Controllers
{
    public abstract class BaseController : Controller
    {
        protected int CurrentUserId
        {
            get
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(claim, out var id) ? id : 0;
            }
        }

        protected string? CurrentRoleCode => User.FindFirstValue(ClaimTypes.Role);

        protected bool IsAdmin => CurrentRoleCode == "Admin";

        protected IPermissionService PermissionService => HttpContext.RequestServices.GetRequiredService<IPermissionService>();
        
        protected bool HasPermission(string code) => PermissionService.CurrentHas(code);
    }
}
