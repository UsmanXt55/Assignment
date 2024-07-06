using Newtonsoft.Json;
namespace BambooCard.CurrencyConversion.Models.DTOs;
public record HistoricalRatesDto(
    [JsonProperty("amount")] double Amount,
    [JsonProperty("base")] string Base,
    bool HasNextPage,
    int PageSize,
    int CurrentPage,
    [JsonProperty("rates")] Dictionary<string, Dictionary<string, double>> Rates);