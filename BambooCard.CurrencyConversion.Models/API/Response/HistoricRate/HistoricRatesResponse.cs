using BambooCard.CurrencyConversion.Models.DTOs;
namespace BambooCard.CurrencyConversion.Models.API.Response.HistoricRate;
public record HistoricRatesResponse(HistoricalRatesDto Conversion) : ApiResponse200Ok("Historic rates data retrieved");