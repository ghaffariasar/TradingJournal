using System.Globalization;
using System.Threading;

namespace TradingProject.Helpers
{
    public static class DateHelper
    {
        private static readonly PersianCalendar PersianCalendar = new();


        public static string ToPersian(DateTime? utcDate, string format = "yyyy/MM/dd HH:mm")
        {
            if (!utcDate.HasValue)
                return string.Empty;

            var dt = utcDate.Value;

            if (dt.Kind == DateTimeKind.Utc)
                dt = dt.ToLocalTime();

            var calendar = new PersianCalendar();

            // بررسی محدوده پشتیبانی‌شده تقویم شمسی
            if (dt < calendar.MinSupportedDateTime || dt > calendar.MaxSupportedDateTime)
            {
                return string.Empty;
            }

            try
            {
                var y = calendar.GetYear(dt);
                var m = calendar.GetMonth(dt);
                var d = calendar.GetDayOfMonth(dt);

                return format
                    .Replace("yyyy", y.ToString("0000"))
                    .Replace("MM", m.ToString("00"))
                    .Replace("dd", d.ToString("00"))
                    .Replace("HH", dt.Hour.ToString("00"))
                    .Replace("mm", dt.Minute.ToString("00"))
                    .Replace("ss", dt.Second.ToString("00"));
            }
            catch (ArgumentOutOfRangeException)
            {
                return string.Empty;
            }
        }

        public static string ToGregorian(DateTime? utcDate, string format = "yyyy-MM-dd HH:mm")
        {
            if (!utcDate.HasValue)
                return string.Empty;

            var dt = utcDate.Value;
            if (dt.Kind == DateTimeKind.Utc)
                dt = dt.ToLocalTime();

            return dt.ToString(format, CultureInfo.InvariantCulture) ;
        }

        public static string ToInputValue(DateTime? utcDate)
        {
            if (!utcDate.HasValue)
                return string.Empty;

            var dt = utcDate.Value;
            if (dt.Kind == DateTimeKind.Utc)
                dt = dt.ToLocalTime();

            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.DateTimeFormat.Calendar = new GregorianCalendar();

            return dt.ToString("yyyy-MM-ddTHH:mm", culture);
        }

        public static int ToPersianYear(this int gregorianYear)
        {
           

            var date = new DateTime(gregorianYear, 1, 1);

            return PersianCalendar.GetYear(date);
        }

        public static int ToPersianMonth(this int gregorianMonth , int gregorianYear)
        {
            if (gregorianMonth is < 1 or > 12)
                throw new ArgumentOutOfRangeException(nameof(gregorianMonth));

            var date = new DateTime(gregorianYear, gregorianMonth, 1);

            return PersianCalendar.GetMonth(date);
        }

        public static DateTime? FromInputValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return DateTime.SpecifyKind(dt, DateTimeKind.Local);
            return null;
        }

        public static string FormatPersianDay(DateTime dt)
        {
            return $"{PersianCalendar.GetYear(dt):0000}/{PersianCalendar.GetMonth(dt):00}/{PersianCalendar.GetDayOfMonth(dt):00}";
        }

        public static string Format(DateTime? utcDate, string format = "yyyy/MM/dd HH:mm")
        {
            if (!utcDate.HasValue)
                return string.Empty;

            var culture = Thread.CurrentThread.CurrentUICulture;
            return culture.Name == "fa-IR"
                ? ToPersian(utcDate, format)
                : ToGregorian(utcDate, format.Replace("/", "-"));
        }
    }
}
