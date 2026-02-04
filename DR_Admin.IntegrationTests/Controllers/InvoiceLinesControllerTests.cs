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
public class InvoiceLinesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public InvoiceLinesControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Invoice Lines Tests

    [Fact]
    [Trait("Category", "InvoiceLines")]
    [Trait("Priority", "1")]
    public async Task GetAllInvoiceLines_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedInvoiceLines();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/InvoiceLines", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceLineDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} invoice lines");
        foreach (var line in result)
        {
            Console.WriteLine($"  - {line.Description}: {line.Quantity} x ${line.UnitPrice} = ${line.TotalPrice}");
        }
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task GetAllInvoiceLines_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedInvoiceLines();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/InvoiceLines", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task GetAllInvoiceLines_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedInvoiceLines();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/InvoiceLines", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task GetAllInvoiceLines_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/InvoiceLines", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Invoice Line By Id Tests

    [Fact]
    [Trait("Category", "InvoiceLines")]
    [Trait("Priority", "2")]
    public async Task GetInvoiceLineById_ValidId_ReturnsOk()
    {
        // Arrange
        var lineId = await SeedInvoiceLines();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/InvoiceLines/{lineId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InvoiceLineDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(lineId, result.Id);

        Console.WriteLine($"Retrieved invoice line: {result.Description}");
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task GetInvoiceLineById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/InvoiceLines/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task GetInvoiceLineById_WithSalesRole_ReturnsOk()
    {
        // Arrange
        var lineId = await SeedInvoiceLines();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/InvoiceLines/{lineId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get Invoice Lines By Invoice Id Tests

    [Fact]
    [Trait("Category", "InvoiceLines")]
    [Trait("Priority", "3")]
    public async Task GetInvoiceLinesByInvoiceId_ValidInvoiceId_ReturnsOk()
    {
        // Arrange
        var invoiceId = await SeedInvoiceWithLines();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/InvoiceLines/invoice/{invoiceId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceLineDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, line => Assert.Equal(invoiceId, line.InvoiceId));

        Console.WriteLine($"Retrieved {result.Count()} invoice lines for invoice {invoiceId}");
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task GetInvoiceLinesByInvoiceId_EmptyInvoice_ReturnsEmptyList()
    {
        // Arrange
        var invoiceId = await SeedEmptyInvoice();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/InvoiceLines/invoice/{invoiceId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceLineDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region Create Invoice Line Tests

    [Fact]
    [Trait("Category", "InvoiceLines")]
    [Trait("Priority", "4")]
    public async Task CreateInvoiceLine_ValidData_ReturnsCreated()
    {
        // Arrange
        await EnsureUnitExists(); // Ensure the "pcs" unit exists
        var invoiceId = await SeedEmptyInvoice();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateInvoiceLineDto
        {
            InvoiceId = invoiceId,
            Description = "Web Hosting - Premium Plan",
            Quantity = 1,
            UnitPrice = 49.99m,
            TaxRate = 0.10m,
            Unit = "pcs",
            TaxAmount = 4.99m,
            TotalPrice = 49.99m,
            TotalWithTax = 54.98m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/InvoiceLines", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InvoiceLineDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(createDto.UnitPrice, result.UnitPrice);

        Console.WriteLine($"Created invoice line with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task CreateInvoiceLine_WithSalesRole_ReturnsCreated()
    {
        // Arrange
        await EnsureUnitExists();
        var invoiceId = await SeedEmptyInvoice();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateInvoiceLineDto
        {
            InvoiceId = invoiceId,
            Description = "Domain Registration",
            Quantity = 1,
            UnitPrice = 14.99m,
            TaxRate = 0.10m,
            Unit = "pcs",
            TaxAmount = 1.49m,
            TotalPrice = 14.99m,
            TotalWithTax = 16.48m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/InvoiceLines", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task CreateInvoiceLine_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        await EnsureUnitExists();
        var invoiceId = await SeedEmptyInvoice();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateInvoiceLineDto
        {
            InvoiceId = invoiceId,
            Description = "Test",
            Quantity = 1,
            UnitPrice = 10.00m,
            Unit = "pcs",
            TaxAmount = 0,
            TotalPrice = 10.00m,
            TotalWithTax = 10.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/InvoiceLines", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update Invoice Line Tests

    [Fact]
    [Trait("Category", "InvoiceLines")]
    [Trait("Priority", "5")]
    public async Task UpdateInvoiceLine_ValidData_ReturnsOk()
    {
        // Arrange
        var (invoiceId, lineId) = await SeedInvoiceLineForUpdate();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceLineDto
        {
            InvoiceId = invoiceId,
            Description = "Updated Service Description",
            Quantity = 2,
            UnitPrice = 59.99m,
            TaxRate = 0.15m,
            Unit = "pcs",
            TaxAmount = 17.99m,
            TotalPrice = 119.98m,
            TotalWithTax = 137.97m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/InvoiceLines/{lineId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InvoiceLineDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(lineId, result.Id);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(updateDto.UnitPrice, result.UnitPrice);

        Console.WriteLine($"Updated invoice line ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task UpdateInvoiceLine_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await EnsureUnitExists();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceLineDto
        {
            InvoiceId = 1,
            Description = "Test",
            Quantity = 1,
            UnitPrice = 10.00m,
            Unit = "pcs",
            TaxAmount = 0,
            TotalPrice = 10.00m,
            TotalWithTax = 10.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/InvoiceLines/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task UpdateInvoiceLine_WithSalesRole_ReturnsOk()
    {
        // Arrange
        var (invoiceId, lineId) = await SeedInvoiceLineForUpdate();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceLineDto
        {
            InvoiceId = invoiceId,
            Description = "Sales Updated Line",
            Quantity = 1,
            UnitPrice = 29.99m,
            Unit = "pcs",
            TaxAmount = 0,
            TotalPrice = 29.99m,
            TotalWithTax = 29.99m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/InvoiceLines/{lineId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task UpdateInvoiceLine_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var (invoiceId, lineId) = await SeedInvoiceLineForUpdate();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceLineDto
        {
            InvoiceId = invoiceId,
            Description = "Test",
            Quantity = 1,
            UnitPrice = 10.00m,
            Unit = "pcs",
            TaxAmount = 0,
            TotalPrice = 10.00m,
            TotalWithTax = 10.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/InvoiceLines/{lineId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Delete Invoice Line Tests

    [Fact]
    [Trait("Category", "InvoiceLines")]
    [Trait("Priority", "6")]
    public async Task DeleteInvoiceLine_ValidId_ReturnsNoContent()
    {
        // Arrange
        var lineId = await SeedInvoiceLines();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/InvoiceLines/{lineId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted invoice line ID: {lineId}");
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task DeleteInvoiceLine_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/InvoiceLines/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "InvoiceLines")]
    public async Task DeleteInvoiceLine_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var lineId = await SeedInvoiceLines();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/InvoiceLines/{lineId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedInvoiceLines()
    {
        var invoiceId = await SeedInvoiceWithLines();
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var line = await context.InvoiceLines.FirstOrDefaultAsync(l => l.InvoiceId == invoiceId);
        return line!.Id;
    }

    private async Task<int> SeedInvoiceWithLines()
    {
        await EnsureUnitExists(); // Ensure unit exists before creating invoice lines
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customerId = await EnsureCustomerExists();
        var unit = await context.Units.FirstOrDefaultAsync(u => u.Code == "pcs");

        var timestamp = DateTime.UtcNow.Ticks;
        var invoice = new Invoice
        {
            CustomerId = customerId,
            InvoiceNumber = $"INV-{timestamp}",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = InvoiceStatus.Draft,
            SubTotal = 99.98m,
            TaxAmount = 9.99m,
            TotalAmount = 109.97m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        var lines = new List<InvoiceLine>
        {
            new InvoiceLine
            {
                InvoiceId = invoice.Id,
                UnitId = unit!.Id,
                Description = "Web Hosting Service",
                Quantity = 1,
                UnitPrice = 49.99m,
                TaxRate = 0.10m,
                TaxAmount = 4.99m,
                TotalPrice = 49.99m,
                TotalWithTax = 54.98m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new InvoiceLine
            {
                InvoiceId = invoice.Id,
                UnitId = unit!.Id,
                Description = "Domain Registration",
                Quantity = 1,
                UnitPrice = 49.99m,
                TaxRate = 0.10m,
                TaxAmount = 4.99m,
                TotalPrice = 49.99m,
                TotalWithTax = 54.98m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.InvoiceLines.AddRange(lines);
        await context.SaveChangesAsync();

        return invoice.Id;
    }

    private async Task<int> SeedEmptyInvoice()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customerId = await EnsureCustomerExists();

        var timestamp = DateTime.UtcNow.Ticks;
        var invoice = new Invoice
        {
            CustomerId = customerId,
            InvoiceNumber = $"INV-EMPTY-{timestamp}",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = InvoiceStatus.Draft,
            SubTotal = 0m,
            TaxAmount = 0m,
            TotalAmount = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        return invoice.Id;
    }

    private async Task<(int invoiceId, int lineId)> SeedInvoiceLineForUpdate()
    {
        await EnsureUnitExists(); // Ensure unit exists before creating invoice line
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var invoiceId = await SeedEmptyInvoice();
        var unit = await context.Units.FirstOrDefaultAsync(u => u.Code == "pcs");

        var line = new InvoiceLine
        {
            InvoiceId = invoiceId,
            UnitId = unit!.Id,
            Description = "Test Line for Update",
            Quantity = 1,
            UnitPrice = 19.99m,
            TaxRate = 0.10m,
            TaxAmount = 1.99m,
            TotalPrice = 19.99m,
            TotalWithTax = 21.98m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.InvoiceLines.Add(line);
        await context.SaveChangesAsync();

        return (invoiceId, line.Id);
    }

    private async Task<int> EnsureCustomerExists()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customer = await context.Customers.FirstOrDefaultAsync();
        if (customer == null)
        {
            var timestamp = DateTime.UtcNow.Ticks;
            customer = new Customer
            {
                Name = $"Test Customer {timestamp}",
                Email = $"customer{timestamp}@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
        }

        return customer.Id;
    }

    private async Task EnsureUnitExists(string unitCode = "pcs")
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var unit = await context.Units.FirstOrDefaultAsync(u => u.Code == unitCode);
        if (unit == null)
        {
            unit = new Unit
            {
                Code = unitCode,
                Name = "Pieces",
                Description = "Standard unit for counting items",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
        }
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Admin");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<string> GetSupportTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Support");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<string> GetSalesTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Sales");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<(string username, string email)> CreateUserWithRole(string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            role = new Role
            {
                Name = roleName,
                Description = $"{roleName} role"
            };
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
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

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };
        await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        Console.WriteLine($"Created {roleName} user: {username}");

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
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login failed for {username}: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        if (result == null || string.IsNullOrEmpty(result.AccessToken))
        {
            throw new Exception($"Failed to get access token for {username}");
        }

        return result.AccessToken;
    }

    #endregion
}

