using System.Net;
namespace BambooCard.CurrencyConversion.Models.API.Response;
public record ApiResponse400BadRequest(string Message, string[] ErrorMessages) : ApiBaseResponse(HttpStatusCode.BadRequest, Message);