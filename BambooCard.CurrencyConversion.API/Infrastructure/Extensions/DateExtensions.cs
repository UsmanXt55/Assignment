namespace BambooCard.CurrencyConversion.API.Infrastructure.Extensions;
public static class DateExtensions
{
    public static string FormatAsDate(this DateOnly date)=> date.ToString("yyyy-MM-dd");
    public static string FormatAsDateString(this DateOnly date) => date.ToString("yyyyMMdd");
}
