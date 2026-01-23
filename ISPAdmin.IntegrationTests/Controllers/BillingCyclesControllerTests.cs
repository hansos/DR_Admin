using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ISPAdmin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class BillingCyclesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly TestWebApplicationFactory _factory;

    public BillingCyclesControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
    }

    #region Get All Billing Cycles Tests

    [Fact]
    [Trait("Category", "BillingCycles")]
    [Trait("Priority", "1")]
    public async Task GetAllBillingCycles_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedBillingCycles();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/BillingCycles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<BillingCycleDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        _output.WriteLine($"Retrieved {result.Count()} billing cycles");
        foreach (var bc in result)
        {
            _output.WriteLine($"  - {bc.Name}: {bc.DurationInDays} days");
        }
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task GetAllBillingCycles_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedBillingCycles();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/BillingCycles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task GetAllBillingCycles_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedBillingCycles();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/BillingCycles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task GetAllBillingCycles_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/BillingCycles");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task GetAllBillingCycles_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/BillingCycles");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Get Billing Cycle By Id Tests

    [Fact]
    [Trait("Category", "BillingCycles")]
    [Trait("Priority", "2")]
    public async Task GetBillingCycleById_ValidId_ReturnsOk()
    {
        // Arrange
        var billingCycleId = await SeedBillingCycles();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/BillingCycles/{billingCycleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<BillingCycleDto>();
        Assert.NotNull(result);
        Assert.Equal(billingCycleId, result.Id);
        Assert.NotEmpty(result.Name);

        _output.WriteLine($"Retrieved billing cycle: {result.Name} ({result.DurationInDays} days)");
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task GetBillingCycleById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/BillingCycles/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task GetBillingCycleById_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/BillingCycles/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Create Billing Cycle Tests

    [Fact]
    [Trait("Category", "BillingCycles")]
    [Trait("Priority", "3")]
    public async Task CreateBillingCycle_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateBillingCycleDto
        {
            Name = "Quarterly",
            DurationInDays = 90,
            Description = "Billed every 3 months"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/BillingCycles", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<BillingCycleDto>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.DurationInDays, result.DurationInDays);
        Assert.Equal(createDto.Description, result.Description);
        Assert.True(result.CreatedAt > DateTime.MinValue);
        Assert.True(result.UpdatedAt > DateTime.MinValue);
        Assert.Equal(result.CreatedAt, result.UpdatedAt, TimeSpan.FromSeconds(1)); // Should be equal when first created

        _output.WriteLine($"Created billing cycle with ID: {result.Id}");
        _output.WriteLine($"  Name: {result.Name}");
        _output.WriteLine($"  Duration: {result.DurationInDays} days");
        _output.WriteLine($"  CreatedAt: {result.CreatedAt}");
        _output.WriteLine($"  UpdatedAt: {result.UpdatedAt}");
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task CreateBillingCycle_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateBillingCycleDto
        {
            Name = "Test Cycle",
            DurationInDays = 30,
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/BillingCycles", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task CreateBillingCycle_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateBillingCycleDto
        {
            Name = "Test Cycle",
            DurationInDays = 30,
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/BillingCycles", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Update Billing Cycle Tests

    [Fact]
    [Trait("Category", "BillingCycles")]
    [Trait("Priority", "4")]
    public async Task UpdateBillingCycle_ValidData_ReturnsOk()
    {
        // Arrange
        var billingCycleId = await SeedBillingCycles();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Get the original billing cycle to compare timestamps
        var getResponse = await _client.GetAsync($"/api/v1/BillingCycles/{billingCycleId}");
        var originalBillingCycle = await getResponse.Content.ReadFromJsonAsync<BillingCycleDto>();
        Assert.NotNull(originalBillingCycle);

        // Wait a bit to ensure UpdatedAt will be different
        await Task.Delay(100);

        var updateDto = new UpdateBillingCycleDto
        {
            Name = "Updated Monthly",
            DurationInDays = 31,
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/BillingCycles/{billingCycleId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<BillingCycleDto>();
        Assert.NotNull(result);
        Assert.Equal(billingCycleId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.DurationInDays, result.DurationInDays);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(originalBillingCycle.CreatedAt, result.CreatedAt); // CreatedAt should not change
        Assert.True(result.UpdatedAt > originalBillingCycle.UpdatedAt); // UpdatedAt should be newer

        _output.WriteLine($"Updated billing cycle ID {result.Id}:");
        _output.WriteLine($"  New Name: {result.Name}");
        _output.WriteLine($"  New Duration: {result.DurationInDays} days");
        _output.WriteLine($"  CreatedAt: {result.CreatedAt} (unchanged)");
        _output.WriteLine($"  UpdatedAt: {result.UpdatedAt} (was: {originalBillingCycle.UpdatedAt})");
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task UpdateBillingCycle_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateBillingCycleDto
        {
            Name = "Updated Name",
            DurationInDays = 30,
            Description = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/BillingCycles/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task UpdateBillingCycle_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var billingCycleId = await SeedBillingCycles();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateBillingCycleDto
        {
            Name = "Updated Name",
            DurationInDays = 30,
            Description = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/BillingCycles/{billingCycleId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task UpdateBillingCycle_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var updateDto = new UpdateBillingCycleDto
        {
            Name = "Updated Name",
            DurationInDays = 30,
            Description = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/BillingCycles/1", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Delete Billing Cycle Tests

    [Fact]
    [Trait("Category", "BillingCycles")]
    [Trait("Priority", "5")]
    public async Task DeleteBillingCycle_ValidId_ReturnsNoContent()
    {
        // Arrange
        var billingCycleId = await SeedBillingCycles();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/BillingCycles/{billingCycleId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _output.WriteLine($"Successfully deleted billing cycle ID: {billingCycleId}");

        // Verify it's actually deleted
        var getResponse = await _client.GetAsync($"/api/v1/BillingCycles/{billingCycleId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task DeleteBillingCycle_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/BillingCycles/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task DeleteBillingCycle_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var billingCycleId = await SeedBillingCycles();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/BillingCycles/{billingCycleId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "BillingCycles")]
    public async Task DeleteBillingCycle_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/v1/BillingCycles/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("Category", "BillingCycles")]
    [Trait("Priority", "6")]
    public async Task FullCRUDFlow_CreateReadUpdateDelete_Success()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        _output.WriteLine("=== Starting Full CRUD Flow for Billing Cycles ===");

        // Create
        _output.WriteLine("\n1. CREATE:");
        var createDto = new CreateBillingCycleDto
        {
            Name = "Bi-Annual",
            DurationInDays = 180,
            Description = "Billed twice per year"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/BillingCycles", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<BillingCycleDto>();
        Assert.NotNull(created);
        _output.WriteLine($"   Created ID: {created.Id}, Name: {created.Name}");

        // Read
        _output.WriteLine("\n2. READ:");
        var readResponse = await _client.GetAsync($"/api/v1/BillingCycles/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);

        var read = await readResponse.Content.ReadFromJsonAsync<BillingCycleDto>();
        Assert.NotNull(read);
        Assert.Equal(created.Id, read.Id);
        Assert.Equal(createDto.Name, read.Name);
        _output.WriteLine($"   Retrieved: {read.Name} ({read.DurationInDays} days)");

        // Update
        _output.WriteLine("\n3. UPDATE:");
        var updateDto = new UpdateBillingCycleDto
        {
            Name = "Semi-Annual",
            DurationInDays = 182,
            Description = "Updated: Billed every 6 months"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/BillingCycles/{created.Id}", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<BillingCycleDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateDto.Name, updated.Name);
        Assert.Equal(updateDto.DurationInDays, updated.DurationInDays);
        _output.WriteLine($"   Updated to: {updated.Name} ({updated.DurationInDays} days)");

        // Delete
        _output.WriteLine("\n4. DELETE:");
        var deleteResponse = await _client.DeleteAsync($"/api/v1/BillingCycles/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        _output.WriteLine($"   Deleted ID: {created.Id}");

        // Verify deletion
        _output.WriteLine("\n5. VERIFY DELETION:");
        var verifyResponse = await _client.GetAsync($"/api/v1/BillingCycles/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, verifyResponse.StatusCode);
        _output.WriteLine("   Confirmed: Resource no longer exists");

        _output.WriteLine("\n=== Full CRUD Flow Completed Successfully ===");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Seeds test billing cycles in the database
    /// Returns the ID of the first seeded billing cycle
    /// </summary>
    private async Task<int> SeedBillingCycles()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var billingCycles = new[]
        {
            new BillingCycle
            {
                Name = "Monthly",
                DurationInDays = 30,
                Description = "Billed monthly",
                CreatedAt = now,
                UpdatedAt = now
            },
            new BillingCycle
            {
                Name = "Yearly",
                DurationInDays = 365,
                Description = "Billed annually",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.BillingCycles.AddRangeAsync(billingCycles);
        await context.SaveChangesAsync();

        return billingCycles[0].Id;
    }

    /// <summary>
    /// Creates a user with Admin role and returns their access token
    /// </summary>
    private async Task<string> GetAdminTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Admin");
        return await LoginAndGetTokenAsync(username);
    }

    /// <summary>
    /// Creates a user with Support role and returns their access token
    /// </summary>
    private async Task<string> GetSupportTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Support");
        return await LoginAndGetTokenAsync(username);
    }

    /// <summary>
    /// Creates a user with Sales role and returns their access token
    /// </summary>
    private async Task<string> GetSalesTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Sales");
        return await LoginAndGetTokenAsync(username);
    }

    /// <summary>
    /// Creates a user with Customer role and returns their access token
    /// </summary>
    private async Task<string> GetCustomerTokenAsync()
    {
        var (username, email) = await CreateUserWithRole("Customer");
        return await LoginAndGetTokenAsync(username);
    }

    /// <summary>
    /// Creates a user with the specified role and returns (username, email)
    /// </summary>
    private async Task<(string username, string email)> CreateUserWithRole(string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure role exists
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

        // Create customer
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

        // Create user
        var username = $"{roleName.ToLower()}user{timestamp}";
        var email = $"{roleName.ToLower()}{timestamp}@example.com";

        var user = new User
        {
            CustomerId = customer.Id,
            Username = username,
            Email = email,
            PasswordHash = "Test@1234", // TODO: In production, use proper password hashing
            EmailConfirmed = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Assign role to user
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };
        await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        _output.WriteLine($"Created {roleName} user: {username}");

        return (username, email);
    }

    /// <summary>
    /// Logs in with the specified username and returns the access token
    /// </summary>
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
