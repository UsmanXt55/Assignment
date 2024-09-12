using BambooCard.CurrencyConversion.API.Infrastructure.Constants;
using BambooCard.CurrencyConversion.API.Infrastructure.Extensions;
using BambooCard.CurrencyConversion.Models;
using BambooCard.CurrencyConversion.Models.DTOs;
using Microsoft.Extensions.Caching.Memory;
namespace BambooCard.CurrencyConversion.API.Services;
public class CachedExchangeRateService(
    IExchangeRateService exchangeRateService,
    IMemoryCache memoryCache,
    IConfiguration configuration
    ) : IExchangeRateService
{
    public async Task<ServiceResult> GetAsync(string baseCurrency, CancellationToken cancellationToken)
    {
        return await exchangeRateService.GetAsync(baseCurrency, cancellationToken);
    }
    public async Task<ServiceResult> GetAsync(string baseCurrency, DateOnly periodStart, DateOnly periodEnd, int pageNumber, CancellationToken cancellationToken)
    {
        HistoricalRatesDto rates;
        var cacheKey = $"key_{baseCurrency}_{periodStart.FormatAsDateString()}_{periodEnd.FormatAsDateString()}_{pageNumber}";
        bool isCacheFound = false;
        if (memoryCache.TryGetValue(cacheKey, out rates!))
            isCacheFound = true;

        if (!isCacheFound)
        {
            var result = await exchangeRateService.GetAsync(baseCurrency, periodStart, periodEnd, pageNumber, cancellationToken);
            if (!result.Success)
                return new(false, result.Message);

            rates = (HistoricalRatesDto)result.Obj!;
            rates = SetPagedData(rates, pageNumber);
            SetCache(cacheKey, rates);
        }
       
        return new(true, "Historical rates fetched", rates);
    }

    public async Task<ServiceResult> ConvertAsync(string fromCurrency, string toCurrency, decimal amount, CancellationToken cancellationToken)
    {
        return await exchangeRateService.ConvertAsync(fromCurrency, toCurrency, amount, cancellationToken);
    }

    private void SetCache(string cacheKey, HistoricalRatesDto dto)
    {
        var cacheExpiryMinutes = configuration.GetSection(ConfigurationConstants.CacheExpiry).Value;

        memoryCache.Set(cacheKey, dto, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(cacheExpiryMinutes is null ? 5 : Convert.ToInt32(cacheExpiryMinutes))
        });
    }
    private HistoricalRatesDto SetPagedData(HistoricalRatesDto rates, int pageNumber)
    {
        bool hasNextPage = false;
        int totalPages = 1;
        int currentPage = 1;

        if (rates.Rates.Count() > 1)
        {
            hasNextPage = true;
            totalPages = rates.Rates.Count();
        }

        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        if (pageNumber > totalPages) pageNumber = totalPages;
        hasNextPage = !(pageNumber == totalPages);
        currentPage = pageNumber;

        Dictionary<string, Dictionary<string, double>> pagedRate = new();
        pagedRate.Add(rates.Rates.ElementAt(currentPage - 1).Key, rates.Rates.ElementAt(currentPage - 1).Value);
        HistoricalRatesDto responseDto = new(rates.Amount, rates.Base, hasNextPage, totalPages, currentPage, pagedRate);
        return responseDto;
    }
}
