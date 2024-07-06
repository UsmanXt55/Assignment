using System.Net;
namespace BambooCard.CurrencyConversion.Models.API.Response;
public record ApiResponse500InternalServerError(string ErrorMessage) : ApiBaseResponse(HttpStatusCode.InternalServerError, ErrorMessage);