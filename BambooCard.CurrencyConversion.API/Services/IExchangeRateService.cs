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
    private readonly IMemoryCache _cache;
    public ExchangeRateService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        _client = httpClientFactory.CreateClient(ApiClientConstants.ExchangeRateService);
        _cache = cache;
        _restrictedCurrencyList = new List<string>()
        {
            "TRY", "PLN", "THB", "MXN"
        };
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
            var cacheKey = $"key_{baseCurrency}_{periodStart.FormatAsDateString()}_{periodEnd.FormatAsDateString()}";
            HistoricalRatesDto rates;
            bool isCacheFound = false;
            if (_cache.TryGetValue(cacheKey, out rates!))
            {
                isCacheFound = true;
            }
            if (!isCacheFound)
            {
                var response = await _client.GetAsync(string.Format("{0}..{1}?from={2}", periodStart.FormatAsDate(), periodEnd.FormatAsDate(), baseCurrency), cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return new(false, "Not found");

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                rates = JsonConvert.DeserializeObject<HistoricalRatesDto>(responseContent)!;

                SetCache(cacheKey, rates!);
            }

            bool hasNextPage = false;
            int totalPages = 1;
            int currentPage = 1;

            if (rates.Rates.Count() > 1)
            {
                hasNextPage = true;
                totalPages = rates.Rates.Count();
            }

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            if (pageNumber > totalPages)
                return new(false, $"Page # {pageNumber} not found");
            hasNextPage = !(pageNumber == totalPages);
            currentPage = pageNumber;

            Dictionary<string, Dictionary<string, double>> pagedRate = new();
            pagedRate.Add(rates.Rates.ElementAt(currentPage - 1).Key, rates.Rates.ElementAt(currentPage - 1).Value);
            HistoricalRatesDto responseDto = new(rates.Amount, rates.Base, hasNextPage, totalPages, currentPage, pagedRate);

            return new(true, "Historical rates fetched", responseDto);
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

    private void SetCache(string cacheKey, HistoricalRatesDto dto)
    {
        _cache.Set(cacheKey, dto, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });
    }
}