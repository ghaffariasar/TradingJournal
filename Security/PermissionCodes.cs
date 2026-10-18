namespace TradingProject.Security
{
    public static class PermissionCodes
    {
        public const string DashboardView = "Dashboard.View";
        public const string TradesViewOwn = "Trades.ViewOwn";
        public const string TradesCreateOwn = "Trades.CreateOwn";
        public const string TradesEditOwn = "Trades.EditOwn";
        public const string TradesDeleteOwn = "Trades.DeleteOwn";
        public const string ReportsViewOwn = "Reports.ViewOwn";
        public const string AccountsManage = "Accounts.Manage";
        public const string LookupsManage = "Lookups.Manage";
        public const string UsersView = "Users.View";
        public const string UsersCreateEdit = "Users.CreateEdit";
        public const string UsersManageAccess = "Users.ManageAccess";
        public const string UsersViewOthersTrades = "Users.ViewOthersTrades";

        public static readonly string[] All =
        [
            DashboardView,
            TradesViewOwn,
            TradesCreateOwn,
            TradesEditOwn,
            TradesDeleteOwn,
            ReportsViewOwn,
            AccountsManage,
            LookupsManage,
            UsersView,
            UsersCreateEdit,
            UsersManageAccess,
            UsersViewOthersTrades
        ];

        public static readonly string[] TraderDefaults =
        [
            DashboardView,
            TradesViewOwn,
            TradesCreateOwn,
            TradesEditOwn,
            TradesDeleteOwn,
            ReportsViewOwn,
            AccountsManage
        ];
    }
}
