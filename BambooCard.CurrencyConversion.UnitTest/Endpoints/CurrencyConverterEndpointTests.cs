using BambooCard.CurrencyConversion.API.Controllers;
using BambooCard.CurrencyConversion.API.Services;
using BambooCard.CurrencyConversion.Models;
using BambooCard.CurrencyConversion.Models.API.Response.Conversion;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace BambooCard.CurrencyConversion.UnitTest.Endpoints;
public class CurrencyConverterEndpointTests
{
    private readonly Mock<IExchangeRateService> _mockService;
    private readonly CurrencyConverterController _controller;
    public CurrencyConverterEndpointTests()
    {
        _mockService = new Mock<IExchangeRateService>();
        _controller = new CurrencyConverterController(_mockService.Object);
    }

    #region Get
    [Fact]
    public async Task Get_ShouldReturn200OkWhenRateDataFound()
    {
        //Arrange
        var baseCurrency = "USD";
        var serviceResultMoq = new ServiceResult(true, "Ok");
        _mockService.Setup(s => s.GetAsync(baseCurrency, CancellationToken.None)).ReturnsAsync(serviceResultMoq);

        //Act
        var result = await _controller.Get(baseCurrency, CancellationToken.None);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);

        Assert.IsType<OkObjectResult>(result);

        var value = Assert.IsType<ConversionResponse>(okResult.Value);
        Assert.NotNull(value);

    }

    [Fact]
    public async Task Get_ShouldReturn400BadRequestWhenValidationFailed()
    {
        //Arrange
        var baseCurrency = "";
        var serviceResultMoq = new ServiceResult(false, "Validation failed");
        _mockService.Setup(s => s.GetAsync(baseCurrency, CancellationToken.None)).ReturnsAsync(serviceResultMoq);

        //Act
        var result = await _controller.Get(baseCurrency, CancellationToken.None);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Get_ShouldReturn400BadRequestWhenRateFetchFailure()
    {
        //Arrange
        var baseCurrency = "";
        var serviceResultMoq = new ServiceResult(false, "Failed to fetch rates");
        _mockService.Setup(s => s.GetAsync(baseCurrency, CancellationToken.None)).ReturnsAsync(serviceResultMoq);

        //Act
        var result = await _controller.Get(baseCurrency, CancellationToken.None);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion

    #region Convert
    [Fact]
    public async Task Convert_ShouldReturn200OkWhenConversionSuccesfull()
    {
        //Arrange
        var fromCurrency = "USD";
        var toCurrency = "GBP";
        var amount = 1m;
        var serviceResultMoq = new ServiceResult(true, "Ok");
        _mockService.Setup(s => s.ConvertAsync(fromCurrency, toCurrency, amount, CancellationToken.None)).ReturnsAsync(serviceResultMoq);

        //Act
        var result = await _controller.Convert(fromCurrency, toCurrency, amount, CancellationToken.None);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Convert_ShouldReturn400BadRequestWhenValidationFailed()
    {
        //Arrange
        var fromCurrency = "USD";
        var toCurrency = "GBP";
        var amount = 1m;
        var serviceResultMoq = new ServiceResult(false, "Validation failed");
        _mockService.Setup(s => s.ConvertAsync(fromCurrency, toCurrency, amount, CancellationToken.None)).ReturnsAsync(serviceResultMoq);

        //Act
        var result = await _controller.Convert(fromCurrency, toCurrency, amount, CancellationToken.None);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
   
    #endregion
}
