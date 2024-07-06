using BambooCard.CurrencyConversion.Models.API.Response;
using Newtonsoft.Json;
namespace BambooCard.CurrencyConversion.API.Infrastructure.Middlewares;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            var errorMessage = JsonConvert.SerializeObject(new ApiResponse500InternalServerError("Internal Server Error"));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(errorMessage);
        }
    }
}