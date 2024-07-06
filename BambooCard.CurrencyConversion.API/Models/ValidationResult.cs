namespace BambooCard.CurrencyConversion.API.Models;
public record ValidationResult(bool Valid, string[]? ErrorMessages = null);