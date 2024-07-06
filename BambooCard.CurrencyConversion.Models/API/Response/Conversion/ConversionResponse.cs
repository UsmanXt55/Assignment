using BambooCard.CurrencyConversion.Models.DTOs;
namespace BambooCard.CurrencyConversion.Models.API.Response.Conversion;
public record ConversionResponse(RatesDto Conversion) : ApiResponse200Ok("Conversion retrieved");