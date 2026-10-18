using System.Threading;

namespace TradingProject.Helpers
{
    public static class LocaleHelper
    {
        public static bool IsPersian =>
            Thread.CurrentThread.CurrentUICulture.Name.StartsWith("fa", StringComparison.OrdinalIgnoreCase);

        public static string Pick(string name, string? nameFa) =>
            IsPersian && !string.IsNullOrWhiteSpace(nameFa) ? nameFa! : name;
    }
}
