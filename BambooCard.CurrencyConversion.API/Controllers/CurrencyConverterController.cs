using BambooCard.CurrencyConversion.API.Models;
using BambooCard.CurrencyConversion.API.Services;
using BambooCard.CurrencyConversion.Models.API.Response;
using BambooCard.CurrencyConversion.Models.API.Response.Conversion;
using BambooCard.CurrencyConversion.Models.API.Response.HistoricRate;
using BambooCard.CurrencyConversion.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace BambooCard.CurrencyConversion.API.Controllers;
[Route("api/v1.0/currency-converter")]
[ApiController]
public class CurrencyConverterController(
    IExchangeRateService exchangeRateService) : ControllerBase
{
    [HttpGet("{baseCurrency}")]
    public async Task<IActionResult> Get(string baseCurrency, CancellationToken cancellationToken)
    {
        var validationResult = ValidateCurrency(baseCurrency);
        if (!validationResult.Valid)
            return BadRequest(new ApiResponse400BadRequest("Validation failed", validationResult.ErrorMessages!));

        var result = await exchangeRateService.GetAsync(baseCurrency, cancellationToken);
        if (!result.Success)
            return BadRequest(new ApiResponse400BadRequest("Failed to fetch rates", [result.Message]));

        var ratesDto = (RatesDto)result.Obj!;
        return Ok(new ConversionResponse(ratesDto));
    }

    [HttpGet("{baseCurrency}/historical")]
    public async Task<IActionResult> GetHistorical(string baseCurrency, string startDate, string endDate, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        var validationResult = ValidateCurrency(baseCurrency);
        if (!validationResult.Valid)
            return BadRequest(new ApiResponse400BadRequest("Validation failed", validationResult.ErrorMessages!));

        validationResult = ValidateDateRange(startDate, endDate);
        if (!validationResult.Valid)
            return BadRequest(new ApiResponse400BadRequest("Validation failed", validationResult.ErrorMessages!));

        var sDate = DateOnly.Parse(startDate);
        var eDate = DateOnly.Parse(endDate);
        var result = await exchangeRateService.GetAsync(baseCurrency, sDate, eDate, pageNumber, cancellationToken);
        if (!result.Success)
            return BadRequest(new ApiResponse400BadRequest("Failed to fetch rates", [result.Message]));

        var ratesDto = (HistoricalRatesDto)result.Obj!;
        return Ok(new HistoricRatesResponse(ratesDto));
    }

    [HttpGet("convert")]
    public async Task<IActionResult> Convert(string from, string to, decimal amount = 1, CancellationToken cancellationToken = default)
    {
        var validationResult = ValidateCurrency(from);
        if (!validationResult.Valid)
            return BadRequest(new ApiResponse400BadRequest("Validation failed", validationResult.ErrorMessages!));

        validationResult = ValidateCurrency(to);
        if (!validationResult.Valid)
            return BadRequest(new ApiResponse400BadRequest("Validation failed", validationResult.ErrorMessages!));

        var result = await exchangeRateService.ConvertAsync(from, to, amount, cancellationToken);
        if (!result.Success)
            return BadRequest(new ApiResponse400BadRequest("Failed to fetch conversion", [result.Message]));

        var ratesDto = (RatesDto)result.Obj!;
        return Ok(new ConversionResponse(ratesDto));
    }

    private ValidationResult ValidateCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return new(false, ["Currency is required"]);
        if (currency.Length != 3)
            return new(false, ["ISO-3 Currency code is required"]);

        return new(true);
    }
    private ValidationResult ValidateDateRange(string startDate, string endDate)
    {
        if (string.IsNullOrWhiteSpace(startDate))
            return new(false, ["Invalid start date provided"]);
        if (string.IsNullOrWhiteSpace(endDate))
            return new(false, ["Invalid date provided"]);

        if(!DateOnly.TryParse(startDate, out var sDate))
            return new(false, ["Invalid start date provided"]);
        if (!DateOnly.TryParse(endDate, out var eDate))
            return new(false, ["Invalid end date provided"]);

        if(eDate < sDate)
            return new(false, ["End date cannot be smaller than start date"]);

        return new(true);
    }
}
