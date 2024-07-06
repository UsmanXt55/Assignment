using System.Net;
namespace BambooCard.CurrencyConversion.Models.API.Response;
public record ApiResponse404NotFound(string Message):ApiBaseResponse(HttpStatusCode.NotFound, Message);