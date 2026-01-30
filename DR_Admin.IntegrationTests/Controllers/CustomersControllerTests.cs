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
public class CustomersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public CustomersControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Customers Tests

    [Fact]
    [Trait("Category", "Customers")]
    [Trait("Priority", "1")]
    public async Task GetAllCustomers_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedCustomers();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Customers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} customers");
        foreach (var customer in result.Take(5))
        {
            Console.WriteLine($"  - {customer.Name} ({customer.Email})");
        }
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task GetAllCustomers_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedCustomers();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Customers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task GetAllCustomers_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedCustomers();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Customers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task GetAllCustomers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Customers");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task GetAllCustomers_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Customers");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Get Customer By Id Tests

    [Fact]
    [Trait("Category", "Customers")]
    [Trait("Priority", "2")]
    public async Task GetCustomerById_ValidId_ReturnsOk()
    {
        // Arrange
        var customerId = await SeedCustomers();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Customers/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.NotEmpty(result.Name);

        Console.WriteLine($"Retrieved customer: {result.Name} ({result.Email})");
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task GetCustomerById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Customers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task GetCustomerById_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Customers/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Create Customer Tests

    [Fact]
    [Trait("Category", "Customers")]
    [Trait("Priority", "3")]
    public async Task CreateCustomer_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCustomerDto
        {
            Name = "Test Customer",
            Email = "testcustomer@example.com",
            Phone = "555-1234",
            Address = "123 Main St",
            City = "Test City",
            State = "TS",
            PostalCode = "12345",
            CountryCode = "US",
            IsCompany = false,
            IsActive = true,
            Status = "Active",
            CreditLimit = 1000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Customers", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Email, result.Email);
        Assert.Equal(createDto.Phone, result.Phone);
        Assert.True(result.CreatedAt > DateTime.MinValue);
        Assert.True(result.UpdatedAt > DateTime.MinValue);

        Console.WriteLine($"Created customer with ID: {result.Id}");
        Console.WriteLine($"  Name: {result.Name}");
        Console.WriteLine($"  Email: {result.Email}");
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task CreateCustomer_WithSalesRole_ReturnsCreated()
    {
        // Arrange
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCustomerDto
        {
            Name = "Sales Customer",
            Email = "salescustomer@example.com",
            Phone = "555-5678",
            Address = "456 Sales St",
            City = "Sales City",
            State = "SC",
            PostalCode = "54321"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Customers", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task CreateCustomer_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCustomerDto
        {
            Name = "Support Customer",
            Email = "supportcustomer@example.com",
            Phone = "555-0000"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Customers", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task CreateCustomer_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            Name = "Test Customer",
            Email = "test@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Customers", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Update Customer Tests

    [Fact]
    [Trait("Category", "Customers")]
    [Trait("Priority", "4")]
    public async Task UpdateCustomer_ValidData_ReturnsOk()
    {
        // Arrange
        var customerId = await SeedCustomers();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateCustomerDto
        {
            Name = "Updated Customer",
            Email = "updated@example.com",
            Phone = "555-9999",
            Address = "999 Updated Ave",
            City = "Updated City",
            State = "UC",
            PostalCode = "99999",
            IsCompany = true,
            CustomerName = "Updated Company",
            IsActive = true,
            Status = "Active",
            CreditLimit = 5000
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Customers/{customerId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Email, result.Email);

        Console.WriteLine($"Updated customer ID {result.Id}:");
        Console.WriteLine($"  New Name: {result.Name}");
        Console.WriteLine($"  New Email: {result.Email}");
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task UpdateCustomer_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateCustomerDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "555-0000"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/Customers/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task UpdateCustomer_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var customerId = await SeedCustomers();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateCustomerDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "555-0000"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Customers/{customerId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task UpdateCustomer_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var updateDto = new UpdateCustomerDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "555-0000"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/Customers/1", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Delete Customer Tests

    [Fact]
    [Trait("Category", "Customers")]
    [Trait("Priority", "5")]
    public async Task DeleteCustomer_ValidId_ReturnsNoContent()
    {
        // Arrange
        var customerId = await SeedCustomersForDelete();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Customers/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Successfully deleted customer ID: {customerId}");

        // Verify it's actually deleted
        var getResponse = await _client.GetAsync($"/api/v1/Customers/{customerId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task DeleteCustomer_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/Customers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task DeleteCustomer_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var customerId = await SeedCustomers();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Customers/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Customers")]
    public async Task DeleteCustomer_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/v1/Customers/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("Category", "Customers")]
    [Trait("Priority", "6")]
    public async Task FullCRUDFlow_CreateReadUpdateDelete_Success()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        Console.WriteLine("=== Starting Full CRUD Flow for Customers ===");

        // Create
        Console.WriteLine("\n1. CREATE:");
        var createDto = new CreateCustomerDto
        {
            Name = "CRUD Test Customer",
            Email = "crudtest@example.com",
            Phone = "555-CRUD",
            Address = "123 CRUD Street",
            City = "CRUD City",
            State = "CC",
            PostalCode = "11111",
            IsCompany = false,
            IsActive = true,
            Status = "Active",
            CreditLimit = 2000
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/Customers", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(created);
        Console.WriteLine($"   Created ID: {created.Id}, Name: {created.Name}");

        // Read
        Console.WriteLine("\n2. READ:");
        var readResponse = await _client.GetAsync($"/api/v1/Customers/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);

        var read = await readResponse.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(read);
        Assert.Equal(created.Id, read.Id);
        Assert.Equal(createDto.Name, read.Name);
        Console.WriteLine($"   Retrieved: {read.Name} ({read.Email})");

        // Update
        Console.WriteLine("\n3. UPDATE:");
        var updateDto = new UpdateCustomerDto
        {
            Name = "CRUD Updated Customer",
            Email = "crudupdated@example.com",
            Phone = "555-UP",
            Address = "456 Updated Ave",
            City = "Update City",
            State = "UC",
            PostalCode = "22222",
            IsCompany = true,
            CustomerName = "CRUD Company Inc.",
            IsActive = true,
            Status = "Active",
            CreditLimit = 5000
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/Customers/{created.Id}", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateDto.Name, updated.Name);
        Assert.Equal(updateDto.Email, updated.Email);
        Console.WriteLine($"   Updated to: {updated.Name} ({updated.Email})");

        // Delete
        Console.WriteLine("\n4. DELETE:");
        var deleteResponse = await _client.DeleteAsync($"/api/v1/Customers/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Console.WriteLine($"   Deleted ID: {created.Id}");

        // Verify deletion
        Console.WriteLine("\n5. VERIFY DELETION:");
        var verifyResponse = await _client.GetAsync($"/api/v1/Customers/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, verifyResponse.StatusCode);
        Console.WriteLine("   Confirmed: Resource no longer exists");

        Console.WriteLine("\n=== Full CRUD Flow Completed Successfully ===");
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedCustomers()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var customers = new[]
        {
            new Customer
            {
                Name = "Test Customer 1",
                Email = "test1@example.com",
                Phone = "555-0001",
                Address = "123 Test St",
                City = "Test City",
                State = "TS",
                PostalCode = "12345",
                IsActive = true,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Customer
            {
                Name = "Test Customer 2",
                Email = "test2@example.com",
                Phone = "555-0002",
                Address = "456 Test Ave",
                City = "Test Town",
                State = "TT",
                PostalCode = "54321",
                IsActive = true,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        return customers[0].Id;
    }

    private async Task<int> SeedCustomersForDelete()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var customer = new Customer
        {
            Name = "Delete Test Customer",
            Email = "delete@example.com",
            Phone = "555-DEL",
            Address = "999 Delete St",
            IsActive = true,
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

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

    private async Task<string> GetCustomerTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Customer");
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

