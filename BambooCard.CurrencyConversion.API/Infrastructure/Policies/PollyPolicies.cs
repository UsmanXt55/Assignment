using Polly;
using System.Net;
namespace BambooCard.CurrencyConversion.API.Infrastructure.Policies;
public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy.HandleResult<HttpResponseMessage>(result =>
        {
            return result.IsSuccessStatusCode == false &&
                   IsTransientError(result.StatusCode);
        })
           .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static bool IsTransientError(HttpStatusCode statusCode)
    {
        int httpStatusCode = (int)statusCode;
        return httpStatusCode >= 500 && httpStatusCode < 600 ||
               httpStatusCode == 429; 
    }
}