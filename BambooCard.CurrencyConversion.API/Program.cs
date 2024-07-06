using BambooCard.CurrencyConversion.API.Infrastructure.Startup;

var builder = WebApplication
    .CreateBuilder(args)
    .RegisterServices();

var app = builder
    .Build()
    .ConfigureMiddleware();

app.Run();
