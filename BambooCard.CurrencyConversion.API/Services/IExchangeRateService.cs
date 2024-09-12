using BambooCard.CurrencyConversion.API.Infrastructure.Constants;
using BambooCard.CurrencyConversion.API.Infrastructure.Extensions;
using BambooCard.CurrencyConversion.Models;
using BambooCard.CurrencyConversion.Models.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
namespace BambooCard.CurrencyConversion.API.Services;
public interface IExchangeRateService
{
    Task<ServiceResult> GetAsync(string baseCurrency, CancellationToken cancellationToken);
    Task<ServiceResult> GetAsync(string baseCurrency, DateOnly periodStart, DateOnly periodEnd, int pageNumber, CancellationToken cancellationToken);
    Task<ServiceResult> ConvertAsync(string fromCurrency, string toCurrency, decimal amount, CancellationToken cancellationToken);
}

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _client;
    private readonly List<string> _restrictedCurrencyList;
    private readonly IConfiguration _configuration;
    public ExchangeRateService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _client = httpClientFactory.CreateClient(ApiClientConstants.ExchangeRateService);
        _configuration = configuration;
        var restrictedCurrencies = configuration.GetSection(ConfigurationConstants.ExcludedCurrencies).Value!.Trim().Split(',');
        _restrictedCurrencyList = new List<string>(restrictedCurrencies);
    }

    public async Task<ServiceResult> GetAsync(string baseCurrency, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _client.GetAsync(string.Format("latest?from={0}", baseCurrency), cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new(false, "Not found");

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var rates = JsonConvert.DeserializeObject<RatesDto>(responseContent);

            return new(true, "Rates fetched", rates);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<ServiceResult> GetAsync(string baseCurrency, DateOnly periodStart, DateOnly periodEnd, int pageNumber, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _client.GetAsync(string.Format("{0}..{1}?from={2}", periodStart.FormatAsDate(), periodEnd.FormatAsDate(), baseCurrency), cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new(false, "Not found");

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var rates = JsonConvert.DeserializeObject<HistoricalRatesDto>(responseContent)!;
            return new(true, "", rates);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<ServiceResult> ConvertAsync(string fromCurrency, string toCurrency, decimal amount, CancellationToken cancellationToken)
    {
        try
        {
            if (_restrictedCurrencyList.Contains(fromCurrency) || _restrictedCurrencyList.Contains(toCurrency))
                return new(false, "Currency conversion for provided currency is not allowed.");

            var response = await _client.GetAsync(string.Format("latest?amount={2}&from={0}&to={1}", fromCurrency, toCurrency, amount), cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new(false, "Not found");

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var rates = JsonConvert.DeserializeObject<RatesDto>(responseContent);

            return new(true, "Conversion fetched", rates);
        }
        catch (Exception)
        {
            throw;
        }
    }

    
}