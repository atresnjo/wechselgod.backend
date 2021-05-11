using System.Globalization;

namespace wechselGod.Api.Extensions
{
    public static class DecimalExtensions
    {
        public static string FormatBalance(this decimal balance) => balance.ToString(CultureInfo.InvariantCulture)
            .Replace("-", "")
            .Replace(",", "").Replace(".", "");
    }
}
