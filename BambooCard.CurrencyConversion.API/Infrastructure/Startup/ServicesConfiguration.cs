using BambooCard.CurrencyConversion.API.Infrastructure.Constants;
using BambooCard.CurrencyConversion.API.Infrastructure.Policies;
using BambooCard.CurrencyConversion.API.Services;
using Microsoft.Extensions.Caching.Memory;
namespace BambooCard.CurrencyConversion.API.Infrastructure.Startup;
public static class ServicesConfiguration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        builder.Configuration.AddJsonFile($"appsettings.{env}.json", false, true);
        builder.Services.AddControllers();
        builder.Services.AddMemoryCache();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var configuration = builder.Configuration;

        builder.Services.AddHttpClient(ApiClientConstants.ExchangeRateService, client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection("AppConfiguration:FxBaseUrl").Value!);
        }).AddPolicyHandler(PollyPolicies.GetRetryPolicy(
            Convert.ToInt32(configuration.GetSection(ConfigurationConstants.FailoverRetryCount).Value)));

        builder.Services.AddScoped<ExchangeRateService>();
        builder.Services.AddScoped<IExchangeRateService>(provider => {
            var exchangeRateService = provider.GetRequiredService<ExchangeRateService>();
            var cache = provider.GetRequiredService<IMemoryCache>();
            return new CachedExchangeRateService(exchangeRateService, cache, configuration);
        });

        return builder;
    }
}
