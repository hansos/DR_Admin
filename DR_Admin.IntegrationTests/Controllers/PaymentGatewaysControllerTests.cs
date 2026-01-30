using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace DR_Admin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class PaymentGatewaysControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public PaymentGatewaysControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Payment Gateways Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "1")]
    public async Task GetAllPaymentGateways_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedPaymentGateways();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentGatewayDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} payment gateways");
        foreach (var gateway in result)
        {
            Console.WriteLine($"  - {gateway.Name} ({gateway.ProviderCode}): Active={gateway.IsActive}, Default={gateway.IsDefault}");
        }
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task GetAllPaymentGateways_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedPaymentGateways();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task GetAllPaymentGateways_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task GetAllPaymentGateways_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Active Payment Gateways Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "2")]
    public async Task GetActivePaymentGateways_ReturnsOnlyActive()
    {
        // Arrange
        await SeedMixedPaymentGateways();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways/active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentGatewayDto>>();
        Assert.NotNull(result);
        Assert.All(result, gateway => Assert.True(gateway.IsActive));

        Console.WriteLine($"Retrieved {result.Count()} active payment gateways");
    }

    #endregion

    #region Get Default Payment Gateway Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "3")]
    public async Task GetDefaultPaymentGateway_ReturnsDefault()
    {
        // Arrange
        await SeedPaymentGateways();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways/default");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaymentGatewayDto>();
        Assert.NotNull(result);
        Assert.True(result.IsDefault);

        Console.WriteLine($"Default payment gateway: {result.Name} ({result.ProviderCode})");
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task GetDefaultPaymentGateway_NoDefault_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways/default");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Get Payment Gateway By Id Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "4")]
    public async Task GetPaymentGatewayById_ValidId_ReturnsOk()
    {
        // Arrange
        var gatewayId = await SeedPaymentGateway("stripe");
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/PaymentGateways/{gatewayId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaymentGatewayDto>();
        Assert.NotNull(result);
        Assert.Equal(gatewayId, result.Id);

        Console.WriteLine($"Retrieved payment gateway: {result.Name}");
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task GetPaymentGatewayById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Get Payment Gateway By Provider Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "5")]
    public async Task GetPaymentGatewayByProvider_ValidProvider_ReturnsOk()
    {
        // Arrange
        await SeedPaymentGateway("stripe");
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways/provider/stripe");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaymentGatewayDto>();
        Assert.NotNull(result);
        Assert.Equal("stripe", result.ProviderCode);

        Console.WriteLine($"Retrieved payment gateway by provider: {result.Name}");
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task GetPaymentGatewayByProvider_InvalidProvider_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/PaymentGateways/provider/unknown");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Create Payment Gateway Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "6")]
    public async Task CreatePaymentGateway_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var timestamp = DateTime.UtcNow.Ticks;
        var createDto = new CreatePaymentGatewayDto
        {
            Name = $"Test Stripe Gateway {timestamp}",
            ProviderCode = "stripe",
            IsActive = true,
            IsDefault = false,
            ApiKey = "sk_test_123456789",
            ApiSecret = "secret_123456789",
            UseSandbox = true,
            Description = "Test Stripe gateway",
            SupportedCurrencies = "USD,EUR,GBP",
            FeePercentage = 2.9m,
            FixedFee = 0.30m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/PaymentGateways", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaymentGatewayDto>();
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.ProviderCode, result.ProviderCode);

        Console.WriteLine($"Created payment gateway ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task CreatePaymentGateway_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreatePaymentGatewayDto
        {
            Name = "Test Gateway",
            ProviderCode = "stripe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/PaymentGateways", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update Payment Gateway Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "7")]
    public async Task UpdatePaymentGateway_ValidData_ReturnsOk()
    {
        // Arrange
        var gatewayId = await SeedPaymentGateway("stripe");
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdatePaymentGatewayDto
        {
            Name = "Updated Stripe Gateway",
            ProviderCode = "stripe",
            IsActive = true,
            IsDefault = false,
            UseSandbox = false,
            Description = "Updated description",
            SupportedCurrencies = "USD,EUR",
            FeePercentage = 3.0m,
            FixedFee = 0.50m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/PaymentGateways/{gatewayId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaymentGatewayDto>();
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.FeePercentage, result.FeePercentage);

        Console.WriteLine($"Updated payment gateway ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task UpdatePaymentGateway_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdatePaymentGatewayDto
        {
            Name = "Updated Gateway",
            ProviderCode = "stripe"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/PaymentGateways/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Set Default Payment Gateway Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "8")]
    public async Task SetDefaultPaymentGateway_ValidId_ReturnsOk()
    {
        // Arrange
        var gatewayId = await SeedPaymentGateway("stripe");
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/v1/PaymentGateways/{gatewayId}/set-default", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Console.WriteLine($"Set payment gateway {gatewayId} as default");
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task SetDefaultPaymentGateway_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/api/v1/PaymentGateways/99999/set-default", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Set Active Status Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "9")]
    public async Task SetPaymentGatewayActiveStatus_ValidId_ReturnsOk()
    {
        // Arrange
        var gatewayId = await SeedPaymentGateway("stripe");
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/PaymentGateways/{gatewayId}/set-active", false);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Console.WriteLine($"Deactivated payment gateway {gatewayId}");
    }

    #endregion

    #region Delete Payment Gateway Tests

    [Fact]
    [Trait("Category", "PaymentGateways")]
    [Trait("Priority", "10")]
    public async Task DeletePaymentGateway_ValidId_ReturnsOk()
    {
        // Arrange
        var gatewayId = await SeedPaymentGateway("stripe");
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/PaymentGateways/{gatewayId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Console.WriteLine($"Deleted payment gateway {gatewayId}");
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task DeletePaymentGateway_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/PaymentGateways/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "PaymentGateways")]
    public async Task DeletePaymentGateway_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var gatewayId = await SeedPaymentGateway("stripe");
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/PaymentGateways/{gatewayId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedPaymentGateway(string providerCode)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var gateway = new PaymentGateway
        {
            Name = $"Test {providerCode} Gateway {timestamp}",
            ProviderCode = providerCode,
            IsActive = true,
            IsDefault = false,
            ApiKey = $"test_key_{timestamp}",
            ApiSecret = $"test_secret_{timestamp}",
            UseSandbox = true,
            Description = $"Test {providerCode} gateway",
            SupportedCurrencies = "USD,EUR,GBP",
            FeePercentage = 2.9m,
            FixedFee = 0.30m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.PaymentGateways.Add(gateway);
        await context.SaveChangesAsync();

        return gateway.Id;
    }

    private async Task SeedPaymentGateways()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var gateways = new List<PaymentGateway>
        {
            new PaymentGateway
            {
                Name = $"Stripe Gateway {timestamp}",
                ProviderCode = "stripe",
                IsActive = true,
                IsDefault = true,
                ApiKey = $"sk_test_{timestamp}",
                ApiSecret = $"secret_{timestamp}",
                UseSandbox = true,
                Description = "Stripe payment gateway",
                SupportedCurrencies = "USD,EUR,GBP",
                FeePercentage = 2.9m,
                FixedFee = 0.30m,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new PaymentGateway
            {
                Name = $"PayPal Gateway {timestamp}",
                ProviderCode = "paypal",
                IsActive = true,
                IsDefault = false,
                ApiKey = $"paypal_client_{timestamp}",
                ApiSecret = $"paypal_secret_{timestamp}",
                UseSandbox = true,
                Description = "PayPal payment gateway",
                SupportedCurrencies = "USD,EUR,GBP",
                FeePercentage = 3.4m,
                FixedFee = 0.30m,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.PaymentGateways.AddRange(gateways);
        await context.SaveChangesAsync();
    }

    private async Task SeedMixedPaymentGateways()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var gateways = new List<PaymentGateway>
        {
            new PaymentGateway
            {
                Name = $"Active Gateway {timestamp}",
                ProviderCode = "stripe",
                IsActive = true,
                IsDefault = true,
                ApiKey = $"sk_test_{timestamp}",
                ApiSecret = $"secret_{timestamp}",
                UseSandbox = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new PaymentGateway
            {
                Name = $"Inactive Gateway {timestamp}",
                ProviderCode = "paypal",
                IsActive = false,
                IsDefault = false,
                ApiKey = $"paypal_client_{timestamp}",
                ApiSecret = $"paypal_secret_{timestamp}",
                UseSandbox = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.PaymentGateways.AddRange(gateways);
        await context.SaveChangesAsync();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        // Ensure the admin user and role exist in the test database
        await EnsureTestUserExists("admin", "Admin@123", "Admin");

        var loginDto = new LoginRequestDto
        {
            Username = "admin",
            Password = "Admin@123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return loginResponse?.AccessToken ?? throw new Exception("Failed to get admin token");
    }

    private async Task<string> GetSupportTokenAsync()
    {
        // Ensure the support user and role exist in the test database
        await EnsureTestUserExists("support", "Support@123", "Support");

        var loginDto = new LoginRequestDto
        {
            Username = "support",
            Password = "Support@123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return loginResponse?.AccessToken ?? throw new Exception("Failed to get support token");
    }

    private async Task<string> GetSalesTokenAsync()
    {
        // Ensure the sales user and role exist in the test database
        await EnsureTestUserExists("sales", "Sales@123", "Sales");

        var loginDto = new LoginRequestDto
        {
            Username = "sales",
            Password = "Sales@123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return loginResponse?.AccessToken ?? throw new Exception("Failed to get sales token");
    }

    #endregion

    private async Task EnsureTestUserExists(string username, string password, string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure role exists
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            role = new ISPAdmin.Data.Entities.Role
            {
                Name = roleName,
                Description = roleName
            };
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }

        // Ensure user exists
        var user = await context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            var customer = new ISPAdmin.Data.Entities.Customer
            {
                Name = $"{username}Customer",
                Email = $"{username}@example.com",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            user = new ISPAdmin.Data.Entities.User
            {
                CustomerId = customer.Id,
                Username = username,
                Email = $"{username}@example.com",
                PasswordHash = password, // tests use plain comparison
                EmailConfirmed = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assign role
            var userRole = new ISPAdmin.Data.Entities.UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            context.UserRoles.Add(userRole);
            await context.SaveChangesAsync();
        }
        else
        {
            // Ensure the user has the role
            var hasRole = await context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
            if (!hasRole)
            {
                context.UserRoles.Add(new ISPAdmin.Data.Entities.UserRole { UserId = user.Id, RoleId = role.Id });
                await context.SaveChangesAsync();
            }

            // Ensure password and active
            if (user.PasswordHash != password || !user.IsActive)
            {
                user.PasswordHash = password;
                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
        }
    }
}

