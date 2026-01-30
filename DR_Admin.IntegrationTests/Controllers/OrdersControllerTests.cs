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
public class OrdersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public OrdersControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Orders Tests

    [Fact]
    [Trait("Category", "Orders")]
    [Trait("Priority", "1")]
    public async Task GetAllOrders_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedOrders();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Orders", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} orders");
    }

    [Fact]
    [Trait("Category", "Orders")]
    public async Task GetAllOrders_WithSalesRole_ReturnsOk()
    {
        // Arrange
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Orders", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Orders")]
    public async Task GetAllOrders_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Orders", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Order By Id Tests

    [Fact]
    [Trait("Category", "Orders")]
    [Trait("Priority", "2")]
    public async Task GetOrderById_ValidId_ReturnsOk()
    {
        // Arrange
        var orderId = await SeedOrders();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Orders/{orderId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<OrderDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    [Trait("Category", "Orders")]
    public async Task GetOrderById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Orders/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Create Order Tests

    [Fact]
    [Trait("Category", "Orders")]
    [Trait("Priority", "3")]
    public async Task CreateOrder_ValidData_ReturnsCreated()
    {
        // Arrange
        var (customerId, serviceId) = await CreateCustomerAndService();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateOrderDto
        {
            CustomerId = customerId,
            ServiceId = serviceId,
            OrderType = OrderType.New,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            NextBillingDate = DateTime.UtcNow.AddMonths(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Orders", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<OrderDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.CustomerId, result.CustomerId);
        Assert.Equal(createDto.ServiceId, result.ServiceId);

        Console.WriteLine($"Created order with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "Orders")]
    public async Task CreateOrder_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var (customerId, serviceId) = await CreateCustomerAndService();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateOrderDto
        {
            CustomerId = customerId,
            ServiceId = serviceId,
            OrderType = OrderType.New,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            NextBillingDate = DateTime.UtcNow.AddMonths(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Orders", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update Order Tests

    [Fact]
    [Trait("Category", "Orders")]
    [Trait("Priority", "4")]
    public async Task UpdateOrder_ValidData_ReturnsOk()
    {
        // Arrange
        var orderId = await SeedOrders();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var order = await context.Orders.FindAsync(new object[] { orderId }, TestContext.Current.CancellationToken);

        var updateDto = new UpdateOrderDto
        {
            ServiceId = order!.ServiceId,
            Status = OrderStatus.Suspended,
            StartDate = order.StartDate,
            EndDate = order.EndDate,
            NextBillingDate = order.NextBillingDate,
            AutoRenew = order.AutoRenew,
            Notes = order.Notes
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Orders/{orderId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<OrderDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Suspended, result.Status);
    }

    [Fact]
    [Trait("Category", "Orders")]
    public async Task UpdateOrder_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var (customerId, serviceId) = await CreateCustomerAndService();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateOrderDto
        {
            ServiceId = serviceId,
            Status = OrderStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            NextBillingDate = DateTime.UtcNow.AddMonths(1),
            AutoRenew = true,
            Notes = string.Empty
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/Orders/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Delete Order Tests

    [Fact]
    [Trait("Category", "Orders")]
    [Trait("Priority", "5")]
    public async Task DeleteOrder_ValidId_ReturnsNoContent()
    {
        // Arrange
        var orderId = await SeedOrdersForDelete();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Orders/{orderId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Successfully deleted order ID: {orderId}");
    }

    [Fact]
    [Trait("Category", "Orders")]
    public async Task DeleteOrder_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/Orders/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedOrders()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (customerId, serviceId) = await CreateCustomerAndService();

        var now = DateTime.UtcNow;
        var orders = new[]
        {
            new Order
            {
                OrderNumber = "ORD-TEST-00001",
                CustomerId = customerId,
                ServiceId = serviceId,
                OrderType = OrderType.New,
                Status = OrderStatus.Active,
                StartDate = now,
                EndDate = now.AddYears(1),
                NextBillingDate = now.AddMonths(1),
                SetupFee = 0,
                RecurringAmount = 100,
                AutoRenew = true,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();

        return orders[0].Id;
    }

    private async Task<int> SeedOrdersForDelete()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (customerId, serviceId) = await CreateCustomerAndService();

        var now = DateTime.UtcNow;
        var order = new Order
        {
            OrderNumber = "ORD-TEST-00002",
            CustomerId = customerId,
            ServiceId = serviceId,
            OrderType = OrderType.New,
            Status = OrderStatus.Active,
            StartDate = now,
            EndDate = now.AddYears(1),
            NextBillingDate = now.AddMonths(1),
            SetupFee = 0,
            RecurringAmount = 100,
            AutoRenew = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        return order.Id;
    }

    private async Task<(int customerId, int serviceId)> CreateCustomerAndService()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customer = new Customer
        {
            Name = "Order Test Customer",
            Email = $"ordertest{DateTime.UtcNow.Ticks}@example.com",
            Phone = "555-ORDER",
            Address = "123 Order St",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        var serviceType = new ServiceType
        {
            Name = "Test Service Type",
            Description = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.ServiceTypes.AddAsync(serviceType);
        await context.SaveChangesAsync();

        var billingCycle = new BillingCycle
        {
            Name = "Monthly",
            DurationInDays = 30,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.BillingCycles.AddAsync(billingCycle);
        await context.SaveChangesAsync();

        var service = new Service
        {
            Name = "Test Service",
            ServiceTypeId = serviceType.Id,
            BillingCycleId = billingCycle.Id,
            Price = 10.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Services.AddAsync(service);
        await context.SaveChangesAsync();

        return (customer.Id, service.Id);
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Admin");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<string> GetSupportTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Support");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<string> GetSalesTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Sales");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<(string username, string email)> CreateUserWithRole(string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            role = new Role { Name = roleName, Description = $"{roleName} role" };
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
        }

        var timestamp = DateTime.UtcNow.Ticks;
        var customer = new Customer
        {
            Name = $"{roleName} Test Customer {timestamp}",
            Email = $"{roleName.ToLower()}{timestamp}@example.com",
            Phone = "555-0100",
            Address = "123 Test St",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

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
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var userRole = new UserRole { UserId = user.Id, RoleId = role.Id };
        await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        return (username, email);
    }

    private async Task<string> LoginAndGetTokenAsync(string username)
    {
        var loginRequest = new LoginRequestDto
        {
            Username = username,
            Password = "Test@1234"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(TestContext.Current.CancellationToken);
        return result!.AccessToken;
    }

    #endregion
}

