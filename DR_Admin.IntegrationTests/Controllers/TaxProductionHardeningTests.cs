using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DR_Admin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class TaxProductionHardeningTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public TaxProductionHardeningTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    [Trait("Category", "Tax")]
    public async Task FinalizeTax_MissingBillingCountryCode_ReturnsBadRequest()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var (customerId, orderId) = await SeedCustomerAndOrderAsync();

        var request = new TaxQuoteRequestDto
        {
            CustomerId = customerId,
            OrderId = orderId,
            BuyerCountryCode = "SE",
            BillingCountryCode = string.Empty,
            IpAddress = "192.168.1.10",
            IdempotencyKey = $"idem-{Guid.NewGuid():N}",
            TaxCurrencyCode = "EUR",
            DisplayCurrencyCode = "EUR",
            Lines = new List<TaxQuoteLineRequestDto>
            {
                new()
                {
                    Description = "Hosting",
                    TaxCategory = "STANDARD",
                    NetAmount = 100m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/tax/finalize", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Tax")]
    public async Task FinalizeTax_ValidRequest_PersistsSnapshotAndEvidence()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var (customerId, orderId) = await SeedCustomerAndOrderAsync();

        var request = new TaxQuoteRequestDto
        {
            CustomerId = customerId,
            OrderId = orderId,
            BuyerCountryCode = "SE",
            BillingCountryCode = "SE",
            IpAddress = "192.168.1.10",
            IdempotencyKey = $"idem-{Guid.NewGuid():N}",
            TaxCurrencyCode = "EUR",
            DisplayCurrencyCode = "EUR",
            Lines = new List<TaxQuoteLineRequestDto>
            {
                new()
                {
                    Description = "Hosting",
                    TaxCategory = "STANDARD",
                    NetAmount = 100m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/tax/finalize", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<TaxQuoteResultDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.SnapshotId.HasValue);
        Assert.True(result.TaxDeterminationEvidenceId.HasValue);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var snapshot = await db.OrderTaxSnapshots.FirstOrDefaultAsync(x => x.Id == result.SnapshotId, TestContext.Current.CancellationToken);
        Assert.NotNull(snapshot);
        Assert.Equal(orderId, snapshot.OrderId);
        Assert.NotNull(snapshot.TaxDeterminationEvidenceId);

        var evidence = await db.TaxDeterminationEvidences.FirstOrDefaultAsync(x => x.Id == snapshot.TaxDeterminationEvidenceId, TestContext.Current.CancellationToken);
        Assert.NotNull(evidence);
        Assert.Equal("SE", evidence.BuyerCountryCode);
        Assert.Equal("SE", evidence.BillingCountryCode);
    }

    [Fact]
    [Trait("Category", "Tax")]
    public async Task OrderTaxSnapshot_UpdateAndDelete_ReturnConflict()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var (_, orderId) = await SeedCustomerAndOrderAsync();
        var snapshotId = await SeedOrderTaxSnapshotAsync(orderId);

        var updateRequest = new UpdateOrderTaxSnapshotDto
        {
            TaxJurisdictionId = null,
            BuyerCountryCode = "SE",
            BuyerStateCode = null,
            BuyerType = CustomerType.B2C,
            BuyerTaxId = string.Empty,
            BuyerTaxIdValidated = false,
            TaxCurrencyCode = "EUR",
            DisplayCurrencyCode = "EUR",
            ExchangeRate = 1m,
            ExchangeRateDate = DateTime.UtcNow,
            NetAmount = 100m,
            TaxAmount = 25m,
            GrossAmount = 125m,
            AppliedTaxRate = 0.25m,
            AppliedTaxName = "VAT",
            ReverseChargeApplied = false,
            RuleVersion = "test",
            CalculationInputsJson = "{}"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/OrderTaxSnapshots/{snapshotId}", updateRequest, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Conflict, updateResponse.StatusCode);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/OrderTaxSnapshots/{snapshotId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);
    }

    [Fact]
    [Trait("Category", "Tax")]
    public async Task FinalizeTax_TrustedFxRequiredWithoutRate_ReturnsBadRequest()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var (customerId, orderId) = await SeedCustomerAndOrderAsync();

        var request = new TaxQuoteRequestDto
        {
            CustomerId = customerId,
            OrderId = orderId,
            BuyerCountryCode = "SE",
            BillingCountryCode = "SE",
            IpAddress = "192.168.1.11",
            IdempotencyKey = $"idem-fx-missing-{Guid.NewGuid():N}",
            TaxCurrencyCode = "EUR",
            DisplayCurrencyCode = "USD",
            RequireTrustedExchangeRate = true,
            Lines = new List<TaxQuoteLineRequestDto>
            {
                new()
                {
                    Description = "Hosting",
                    TaxCategory = "STANDARD",
                    NetAmount = 100m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/tax/finalize", request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Tax")]
    public async Task FinalizeTax_TrustedFxRateExists_ReturnsOkAndPersistsFxSource()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var (customerId, orderId) = await SeedCustomerAndOrderAsync();
        await SeedExchangeRateAsync("EUR", "USD", 1.10m, CurrencyRateSource.ECB);

        var request = new TaxQuoteRequestDto
        {
            CustomerId = customerId,
            OrderId = orderId,
            BuyerCountryCode = "SE",
            BillingCountryCode = "SE",
            IpAddress = "192.168.1.12",
            IdempotencyKey = $"idem-fx-ok-{Guid.NewGuid():N}",
            TaxCurrencyCode = "EUR",
            DisplayCurrencyCode = "USD",
            RequireTrustedExchangeRate = true,
            Lines = new List<TaxQuoteLineRequestDto>
            {
                new()
                {
                    Description = "Hosting",
                    TaxCategory = "STANDARD",
                    NetAmount = 100m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/tax/finalize", request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<TaxQuoteResultDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(CurrencyRateSource.ECB, result.ExchangeRateSource);
        Assert.Equal(1.10m, result.ExchangeRate);
    }

    [Fact]
    [Trait("Category", "Tax")]
    public async Task FinalizeTax_SameIdempotencyKey_ReturnsSameSnapshot()
    {
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var (customerId, orderId) = await SeedCustomerAndOrderAsync();
        var idem = $"idem-repeat-{Guid.NewGuid():N}";

        var request = new TaxQuoteRequestDto
        {
            CustomerId = customerId,
            OrderId = orderId,
            BuyerCountryCode = "SE",
            BillingCountryCode = "SE",
            IpAddress = "192.168.1.13",
            IdempotencyKey = idem,
            TaxCurrencyCode = "EUR",
            DisplayCurrencyCode = "EUR",
            Lines = new List<TaxQuoteLineRequestDto>
            {
                new()
                {
                    Description = "Hosting",
                    TaxCategory = "STANDARD",
                    NetAmount = 100m
                }
            }
        };

        var response1 = await _client.PostAsJsonAsync("/api/v1/tax/finalize", request, TestContext.Current.CancellationToken);
        var response2 = await _client.PostAsJsonAsync("/api/v1/tax/finalize", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        var result1 = await response1.Content.ReadFromJsonAsync<TaxQuoteResultDto>(TestContext.Current.CancellationToken);
        var result2 = await response2.Content.ReadFromJsonAsync<TaxQuoteResultDto>(TestContext.Current.CancellationToken);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.SnapshotId, result2.SnapshotId);
    }

    private async Task<(int customerId, int orderId)> SeedCustomerAndOrderAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customer = new Customer
        {
            Name = $"Tax Test Customer {DateTime.UtcNow.Ticks}",
            Email = $"tax-customer-{DateTime.UtcNow.Ticks}@example.com",
            Phone = "123456",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var order = new Order
        {
            OrderNumber = $"ORD-TAX-{DateTime.UtcNow.Ticks}",
            CustomerId = customer.Id,
            OrderType = OrderType.New,
            Status = OrderStatus.Pending,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            NextBillingDate = DateTime.UtcNow.AddMonths(1),
            CurrencyCode = "EUR",
            BaseCurrencyCode = "EUR",
            SetupFee = 0,
            RecurringAmount = 100,
            DiscountAmount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (customer.Id, order.Id);
    }

    private async Task<int> SeedOrderTaxSnapshotAsync(int orderId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var snapshot = new OrderTaxSnapshot
        {
            OrderId = orderId,
            BuyerCountryCode = "SE",
            BuyerType = CustomerType.B2C,
            BuyerTaxId = string.Empty,
            BuyerTaxIdValidated = false,
            TaxCurrencyCode = "EUR",
            DisplayCurrencyCode = "EUR",
            NetAmount = 100m,
            TaxAmount = 25m,
            GrossAmount = 125m,
            AppliedTaxRate = 0.25m,
            AppliedTaxName = "VAT",
            ReverseChargeApplied = false,
            RuleVersion = "seed",
            CalculationInputsJson = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.OrderTaxSnapshots.Add(snapshot);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return snapshot.Id;
    }

    private async Task SeedExchangeRateAsync(string baseCurrency, string targetCurrency, decimal rate, CurrencyRateSource source)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var exchangeRate = new CurrencyExchangeRate
        {
            BaseCurrency = baseCurrency,
            TargetCurrency = targetCurrency,
            Rate = rate,
            EffectiveRate = rate,
            EffectiveDate = DateTime.UtcNow.AddMinutes(-30),
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            Source = source,
            IsActive = true,
            Markup = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.CurrencyExchangeRates.Add(exchangeRate);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Admin");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<(string username, string email)> CreateUserWithRole(string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, TestContext.Current.CancellationToken);
        if (role == null)
        {
            role = new Role
            {
                Name = roleName,
                Description = $"{roleName} role"
            };
            await context.Roles.AddAsync(role, TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var timestamp = DateTime.UtcNow.Ticks;
        var customer = new Customer
        {
            Name = $"{roleName} Test Customer {timestamp}",
            Email = $"{roleName.ToLower()}{timestamp}@example.com",
            Phone = "555-0100",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var username = $"{roleName.ToLower()}user{timestamp}";
        var email = $"{roleName.ToLower()}{timestamp}@example.com";

        var user = new User
        {
            CustomerId = customer.Id,
            Username = username,
            Email = email,
            PasswordHash = "Test@1234",
            EmailConfirmed = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Users.AddAsync(user, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };
        await context.UserRoles.AddAsync(userRole, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (username, email);
    }

    private async Task<string> LoginAndGetTokenAsync(string username)
    {
        var loginRequest = new LoginRequestDto
        {
            Username = username,
            Password = "Test@1234"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));

        return result.AccessToken;
    }
}
