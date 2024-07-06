using Newtonsoft.Json;
namespace BambooCard.CurrencyConversion.Models.DTOs;
public record RatesDto(
    [JsonProperty("amount")] double Amount,
    [JsonProperty("base")] string Base,
    [JsonProperty("date")] DateTime Date,
    [JsonProperty("rates")] Dictionary<string, double> Rates);