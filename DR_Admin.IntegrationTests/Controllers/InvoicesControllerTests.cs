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
public class InvoicesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public InvoicesControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Invoices Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "1")]
    public async Task GetAllInvoices_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedInvoices();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Invoices", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} invoices");
        foreach (var invoice in result)
        {
            Console.WriteLine($"  - {invoice.InvoiceNumber}: ${invoice.TotalAmount} ({invoice.Status})");
        }
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task GetAllInvoices_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedInvoices();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Invoices", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task GetAllInvoices_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedInvoices();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Invoices", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task GetAllInvoices_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Invoices", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Invoices By Customer Id Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "2")]
    public async Task GetInvoicesByCustomerId_ValidCustomerId_ReturnsOk()
    {
        // Arrange
        var customerId = await SeedCustomerWithInvoices();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Invoices/customer/{customerId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, invoice => Assert.Equal(customerId, invoice.CustomerId));

        Console.WriteLine($"Retrieved {result.Count()} invoices for customer {customerId}");
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task GetInvoicesByCustomerId_EmptyCustomer_ReturnsEmptyList()
    {
        // Arrange
        var customerId = await EnsureCustomerExists();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Invoices/customer/{customerId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
    }

    #endregion

    #region Get Invoices By Status Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "3")]
    public async Task GetInvoicesByStatus_DraftStatus_ReturnsOnlyDrafts()
    {
        // Arrange
        await SeedMixedStatusInvoices();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Invoices/status/{InvoiceStatus.Draft}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.All(result, invoice => Assert.Equal(InvoiceStatus.Draft, invoice.Status));

        Console.WriteLine($"Retrieved {result.Count()} draft invoices");
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task GetInvoicesByStatus_PaidStatus_ReturnsOnlyPaid()
    {
        // Arrange
        await SeedMixedStatusInvoices();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Invoices/status/{InvoiceStatus.Paid}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<InvoiceDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.All(result, invoice => Assert.Equal(InvoiceStatus.Paid, invoice.Status));

        Console.WriteLine($"Retrieved {result.Count()} paid invoices");
    }

    #endregion

    #region Get Invoice By Id Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "4")]
    public async Task GetInvoiceById_ValidId_ReturnsOk()
    {
        // Arrange
        var invoiceId = await SeedInvoices();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Invoices/{invoiceId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InvoiceDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(invoiceId, result.Id);

        Console.WriteLine($"Retrieved invoice: {result.InvoiceNumber}");
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task GetInvoiceById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Invoices/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Get Invoice By Number Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "5")]
    public async Task GetInvoiceByNumber_ValidNumber_ReturnsOk()
    {
        // Arrange
        var (invoiceId, invoiceNumber) = await SeedInvoiceWithNumber();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Invoices/number/{invoiceNumber}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InvoiceDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(invoiceNumber, result.InvoiceNumber);

        Console.WriteLine($"Retrieved invoice by number: {result.InvoiceNumber}");
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task GetInvoiceByNumber_InvalidNumber_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Invoices/number/INVALID-999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Create Invoice Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "6")]
    public async Task CreateInvoice_ValidData_ReturnsCreated()
    {
        // Arrange
        var customerId = await EnsureCustomerExists();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var timestamp = DateTime.UtcNow.Ticks;
        var createDto = new CreateInvoiceDto
        {
            CustomerId = customerId,
            InvoiceNumber = $"INV-TEST-{timestamp}",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = InvoiceStatus.Draft,
            SubTotal = 100.00m,
            TaxAmount = 10.00m,
            TotalAmount = 110.00m,
            Notes = "Test invoice"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Invoices", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InvoiceDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.InvoiceNumber, result.InvoiceNumber);
        Assert.Equal(createDto.TotalAmount, result.TotalAmount);

        Console.WriteLine($"Created invoice with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task CreateInvoice_WithSalesRole_ReturnsCreated()
    {
        // Arrange
        var customerId = await EnsureCustomerExists();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var timestamp = DateTime.UtcNow.Ticks;
        var createDto = new CreateInvoiceDto
        {
            CustomerId = customerId,
            InvoiceNumber = $"INV-SALES-{timestamp}",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = InvoiceStatus.Draft,
            SubTotal = 50.00m,
            TaxAmount = 5.00m,
            TotalAmount = 55.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Invoices", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task CreateInvoice_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var customerId = await EnsureCustomerExists();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateInvoiceDto
        {
            CustomerId = customerId,
            InvoiceNumber = "INV-TEST",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = InvoiceStatus.Draft,
            TotalAmount = 100.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Invoices", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update Invoice Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "7")]
    public async Task UpdateInvoice_ValidData_ReturnsOk()
    {
        // Arrange
        var invoiceId = await SeedInvoices();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceDto
        {
            Status = InvoiceStatus.Issued,
            SubTotal = 150.00m,
            TaxAmount = 15.00m,
            TotalAmount = 165.00m,
            Notes = "Updated invoice",
            DueDate = DateTime.UtcNow.AddDays(45)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Invoices/{invoiceId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InvoiceDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(invoiceId, result.Id);
        Assert.Equal(updateDto.Status, result.Status);
        Assert.Equal(updateDto.TotalAmount, result.TotalAmount);

        Console.WriteLine($"Updated invoice ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task UpdateInvoice_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceDto
        {
            Status = InvoiceStatus.Draft,
            TotalAmount = 100.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/Invoices/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task UpdateInvoice_WithSalesRole_ReturnsOk()
    {
        // Arrange
        var invoiceId = await SeedInvoices();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceDto
        {
            Status = InvoiceStatus.Issued,
            TotalAmount = 100.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Invoices/{invoiceId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task UpdateInvoice_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var invoiceId = await SeedInvoices();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateInvoiceDto
        {
            Status = InvoiceStatus.Draft,
            TotalAmount = 100.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Invoices/{invoiceId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Delete Invoice Tests

    [Fact]
    [Trait("Category", "Invoices")]
    [Trait("Priority", "8")]
    public async Task DeleteInvoice_ValidId_ReturnsNoContent()
    {
        // Arrange
        var invoiceId = await SeedInvoices();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Invoices/{invoiceId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted invoice ID: {invoiceId}");
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task DeleteInvoice_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/Invoices/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Invoices")]
    public async Task DeleteInvoice_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var invoiceId = await SeedInvoices();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Invoices/{invoiceId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedInvoices()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customerId = await EnsureCustomerExists();

        var timestamp = DateTime.UtcNow.Ticks;
        var invoice = new Invoice
        {
            CustomerId = customerId,
            InvoiceNumber = $"INV-{timestamp}",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = InvoiceStatus.Draft,
            SubTotal = 100.00m,
            TaxAmount = 10.00m,
            TotalAmount = 110.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        return invoice.Id;
    }

    private async Task<int> SeedCustomerWithInvoices()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var customer = new Customer
        {
            Name = $"Invoice Test Customer {timestamp}",
            Email = $"invoicetest{timestamp}@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var invoices = new List<Invoice>
        {
            new Invoice
            {
                CustomerId = customer.Id,
                InvoiceNumber = $"INV-1-{timestamp}",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = InvoiceStatus.Draft,
                SubTotal = 100.00m,
                TaxAmount = 10.00m,
                TotalAmount = 110.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Invoice
            {
                CustomerId = customer.Id,
                InvoiceNumber = $"INV-2-{timestamp}",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = InvoiceStatus.Issued,
                SubTotal = 200.00m,
                TaxAmount = 20.00m,
                TotalAmount = 220.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Invoices.AddRange(invoices);
        await context.SaveChangesAsync();

        return customer.Id;
    }

    private async Task SeedMixedStatusInvoices()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customerId = await EnsureCustomerExists();

        var timestamp = DateTime.UtcNow.Ticks;
        var invoices = new List<Invoice>
        {
            new Invoice
            {
                CustomerId = customerId,
                InvoiceNumber = $"INV-DRAFT-{timestamp}",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = InvoiceStatus.Draft,
                SubTotal = 100.00m,
                TaxAmount = 10.00m,
                TotalAmount = 110.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Invoice
            {
                CustomerId = customerId,
                InvoiceNumber = $"INV-PAID-{timestamp}",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = InvoiceStatus.Paid,
                SubTotal = 200.00m,
                TaxAmount = 20.00m,
                TotalAmount = 220.00m,
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Invoice
            {
                CustomerId = customerId,
                InvoiceNumber = $"INV-SENT-{timestamp}",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = InvoiceStatus.Issued,
                SubTotal = 150.00m,
                TaxAmount = 15.00m,
                TotalAmount = 165.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Invoices.AddRange(invoices);
        await context.SaveChangesAsync();
    }

    private async Task<(int invoiceId, string invoiceNumber)> SeedInvoiceWithNumber()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customerId = await EnsureCustomerExists();

        var timestamp = DateTime.UtcNow.Ticks;
        var invoiceNumber = $"INV-NUMBER-{timestamp}";
        
        var invoice = new Invoice
        {
            CustomerId = customerId,
            InvoiceNumber = invoiceNumber,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = InvoiceStatus.Draft,
            SubTotal = 100.00m,
            TaxAmount = 10.00m,
            TotalAmount = 110.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        return (invoice.Id, invoiceNumber);
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

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(TestContext.Current.CancellationToken);
        if (result == null || string.IsNullOrEmpty(result.AccessToken))
        {
            throw new Exception($"Failed to get access token for {username}");
        }

        return result.AccessToken;
    }

    #endregion
}

