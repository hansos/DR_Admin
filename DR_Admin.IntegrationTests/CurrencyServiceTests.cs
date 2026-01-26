using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DR_Admin.IntegrationTests;

/// <summary>
/// Integration tests for the CurrencyService
/// </summary>
[Collection("Sequential")]
public class CurrencyServiceTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly ICurrencyService _currencyService;
    private readonly TestWebApplicationFactory _factory;

    public CurrencyServiceTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        var scope = factory.Services.CreateScope();
        _currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();
    }

    /// <summary>
    /// Tests creating a new currency exchange rate
    /// </summary>
    [Fact]
    public async Task CreateRate_ValidData_ReturnsCreatedRate()
    {
        // Arrange
        var createDto = new CreateCurrencyExchangeRateDto
        {
            BaseCurrency = "EUR",
            TargetCurrency = "USD",
            Rate = 1.10m,
            EffectiveDate = DateTime.UtcNow,
            Source = CurrencyRateSource.Manual,
            IsActive = true,
            Markup = 0m
        };

        // Act
        var result = await _currencyService.CreateRateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("EUR", result.BaseCurrency);
        Assert.Equal("USD", result.TargetCurrency);
        Assert.Equal(1.10m, result.Rate);
        Assert.Equal(1.10m, result.EffectiveRate);
        Assert.True(result.IsActive);
    }

    /// <summary>
    /// Tests creating a rate with markup
    /// </summary>
    [Fact]
    public async Task CreateRate_WithMarkup_CalculatesEffectiveRate()
    {
        // Arrange
        var createDto = new CreateCurrencyExchangeRateDto
        {
            BaseCurrency = "EUR",
            TargetCurrency = "GBP",
            Rate = 0.85m,
            EffectiveDate = DateTime.UtcNow,
            Source = CurrencyRateSource.Manual,
            IsActive = true,
            Markup = 2.5m // 2.5% markup
        };

        // Act
        var result = await _currencyService.CreateRateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.85m, result.Rate);
        Assert.Equal(2.5m, result.Markup);
        Assert.Equal(0.87125m, result.EffectiveRate); // 0.85 * 1.025 = 0.87125
    }

    /// <summary>
    /// Tests retrieving all rates
    /// </summary>
    [Fact]
    public async Task GetAllRates_ReturnsAllRates()
    {
        // Arrange
        await CreateTestRate("EUR", "USD", 1.10m);
        await CreateTestRate("EUR", "GBP", 0.85m);

        // Act
        var results = await _currencyService.GetAllRatesAsync();

        // Assert
        Assert.NotNull(results);
        Assert.True(results.Count() >= 2);
    }

    /// <summary>
    /// Tests retrieving active rates only
    /// </summary>
    [Fact]
    public async Task GetActiveRates_ReturnsOnlyActiveRates()
    {
        // Arrange
        var activeRate = await CreateTestRate("EUR", "USD", 1.10m, true);
        var inactiveRate = await CreateTestRate("EUR", "JPY", 130.0m, false);

        // Act
        var results = await _currencyService.GetActiveRatesAsync();

        // Assert
        Assert.NotNull(results);
        Assert.Contains(results, r => r.Id == activeRate.Id);
        Assert.DoesNotContain(results, r => r.Id == inactiveRate.Id);
    }

    /// <summary>
    /// Tests getting exchange rate between two currencies
    /// </summary>
    [Fact]
    public async Task GetExchangeRate_ValidCurrencies_ReturnsRate()
    {
        // Arrange
        await CreateTestRate("EUR", "USD", 1.10m);

        // Act
        var result = await _currencyService.GetExchangeRateAsync("EUR", "USD");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EUR", result.BaseCurrency);
        Assert.Equal("USD", result.TargetCurrency);
        Assert.Equal(1.10m, result.Rate);
    }

    /// <summary>
    /// Tests getting exchange rate for same currency returns 1
    /// </summary>
    [Fact]
    public async Task GetExchangeRate_SameCurrency_ReturnsRateOfOne()
    {
        // Act
        var result = await _currencyService.GetExchangeRateAsync("EUR", "EUR");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1.0m, result.Rate);
        Assert.Equal(1.0m, result.EffectiveRate);
    }

    /// <summary>
    /// Tests converting currency
    /// </summary>
    [Fact]
    public async Task ConvertCurrency_ValidData_ReturnsConvertedAmount()
    {
        // Arrange
        await CreateTestRate("EUR", "USD", 1.10m);

        // Act
        var result = await _currencyService.ConvertCurrencyAsync(100m, "EUR", "USD");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100m, result.OriginalAmount);
        Assert.Equal("EUR", result.FromCurrency);
        Assert.Equal("USD", result.ToCurrency);
        Assert.Equal(1.10m, result.ExchangeRate);
        Assert.Equal(110m, result.ConvertedAmount);
    }

    /// <summary>
    /// Tests converting currency with markup
    /// </summary>
    [Fact]
    public async Task ConvertCurrency_WithMarkup_UsesEffectiveRate()
    {
        // Arrange
        var createDto = new CreateCurrencyExchangeRateDto
        {
            BaseCurrency = "EUR",
            TargetCurrency = "CHF",
            Rate = 0.95m,
            EffectiveDate = DateTime.UtcNow,
            Source = CurrencyRateSource.Manual,
            IsActive = true,
            Markup = 5m // 5% markup
        };
        await _currencyService.CreateRateAsync(createDto);

        // Act
        var result = await _currencyService.ConvertCurrencyAsync(100m, "EUR", "CHF");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.9975m, result.ExchangeRate); // 0.95 * 1.05 = 0.9975
        Assert.Equal(99.75m, result.ConvertedAmount); // 100 * 0.9975 = 99.75
    }

    /// <summary>
    /// Tests converting currency without rate throws exception
    /// </summary>
    [Fact]
    public async Task ConvertCurrency_NoRate_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _currencyService.ConvertCurrencyAsync(100m, "EUR", "XXX"));
    }

    /// <summary>
    /// Tests updating a rate
    /// </summary>
    [Fact]
    public async Task UpdateRate_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        var createdRate = await CreateTestRate("EUR", "CAD", 1.45m);
        var updateDto = new UpdateCurrencyExchangeRateDto
        {
            Rate = 1.50m,
            EffectiveDate = createdRate.EffectiveDate,
            Source = CurrencyRateSource.ECB,
            IsActive = true,
            Markup = 1.0m
        };

        // Act
        var result = await _currencyService.UpdateRateAsync(createdRate.Id, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1.50m, result.Rate);
        Assert.Equal(CurrencyRateSource.ECB, result.Source);
        Assert.Equal(1.0m, result.Markup);
        Assert.Equal(1.515m, result.EffectiveRate); // 1.50 * 1.01 = 1.515
    }

    /// <summary>
    /// Tests deleting a rate
    /// </summary>
    [Fact]
    public async Task DeleteRate_ExistingRate_DeletesSuccessfully()
    {
        // Arrange
        var createdRate = await CreateTestRate("EUR", "AUD", 1.65m);

        // Act
        var result = await _currencyService.DeleteRateAsync(createdRate.Id);

        // Assert
        Assert.True(result);
        
        // Verify it's deleted
        var deletedRate = await _currencyService.GetRateByIdAsync(createdRate.Id);
        Assert.Null(deletedRate);
    }

    /// <summary>
    /// Tests deactivating expired rates
    /// </summary>
    [Fact]
    public async Task DeactivateExpiredRates_DeactivatesExpiredOnes()
    {
        // Arrange
        var expiredRateDto = new CreateCurrencyExchangeRateDto
        {
            BaseCurrency = "EUR",
            TargetCurrency = "NZD",
            Rate = 1.75m,
            EffectiveDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddDays(-1), // Expired yesterday
            Source = CurrencyRateSource.Manual,
            IsActive = true,
            Markup = 0m
        };
        await _currencyService.CreateRateAsync(expiredRateDto);

        // Act
        var count = await _currencyService.DeactivateExpiredRatesAsync();

        // Assert
        Assert.True(count >= 1);
    }

    /// <summary>
    /// Tests getting rates for currency pair
    /// </summary>
    [Fact]
    public async Task GetRatesForCurrencyPair_ReturnsAllPairRates()
    {
        // Arrange
        await CreateTestRate("EUR", "SEK", 11.0m);
        await CreateTestRate("EUR", "SEK", 11.5m); // Second rate for same pair
        await CreateTestRate("EUR", "NOK", 11.2m); // Different pair

        // Act
        var results = await _currencyService.GetRatesForCurrencyPairAsync("EUR", "SEK");

        // Assert
        Assert.NotNull(results);
        Assert.True(results.Count() >= 2);
        Assert.All(results, r =>
        {
            Assert.Equal("EUR", r.BaseCurrency);
            Assert.Equal("SEK", r.TargetCurrency);
        });
    }

    // Helper method to create test rates
    private async Task<CurrencyExchangeRateDto> CreateTestRate(
        string baseCurrency, 
        string targetCurrency, 
        decimal rate, 
        bool isActive = true)
    {
        var createDto = new CreateCurrencyExchangeRateDto
        {
            BaseCurrency = baseCurrency,
            TargetCurrency = targetCurrency,
            Rate = rate,
            EffectiveDate = DateTime.UtcNow,
            Source = CurrencyRateSource.Manual,
            IsActive = isActive,
            Markup = 0m
        };

        return await _currencyService.CreateRateAsync(createDto);
    }
}
