using System.Net;
namespace BambooCard.CurrencyConversion.Models.API.Response;
public record ApiResponse200Ok(string Message) : ApiBaseResponse(HttpStatusCode.OK, Message);