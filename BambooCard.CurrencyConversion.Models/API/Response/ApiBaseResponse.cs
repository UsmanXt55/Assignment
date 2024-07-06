using System.Net;
namespace BambooCard.CurrencyConversion.Models.API.Response;
public record ApiBaseResponse(HttpStatusCode Code, string Message);
