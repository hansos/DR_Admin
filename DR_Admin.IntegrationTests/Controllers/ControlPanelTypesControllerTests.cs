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

namespace DR_Admin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class ControlPanelTypesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly TestWebApplicationFactory _factory;

    public ControlPanelTypesControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
    }

    #region Get All Control Panel Types Tests

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "1")]
    public async Task GetAllControlPanelTypes_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedControlPanelTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ControlPanelTypeDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        _output.WriteLine($"Retrieved {result.Count()} control panel types");
        foreach (var type in result)
        {
            _output.WriteLine($"  - {type.DisplayName} ({type.Name}): Active={type.IsActive}");
        }
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetAllControlPanelTypes_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedControlPanelTypes();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetAllControlPanelTypes_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedControlPanelTypes();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetAllControlPanelTypes_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetAllControlPanelTypes_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Get Active Control Panel Types Tests

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "1")]
    public async Task GetActiveControlPanelTypes_WithAdminRole_ReturnsOnlyActive()
    {
        // Arrange
        await SeedControlPanelTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes/active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ControlPanelTypeDto>>();
        Assert.NotNull(result);
        Assert.All(result, type => Assert.True(type.IsActive));

        _output.WriteLine($"Retrieved {result.Count()} active control panel types");
        foreach (var type in result)
        {
            _output.WriteLine($"  - {type.DisplayName} ({type.Name})");
        }
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetActiveControlPanelTypes_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedControlPanelTypes();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes/active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetActiveControlPanelTypes_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes/active");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Control Panel Type By Id Tests

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "2")]
    public async Task GetControlPanelTypeById_ValidId_ReturnsOk()
    {
        // Arrange
        var controlPanelTypeId = await SeedControlPanelTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ControlPanelTypeDto>();
        Assert.NotNull(result);
        Assert.Equal(controlPanelTypeId, result.Id);
        Assert.NotEmpty(result.Name);
        Assert.NotEmpty(result.DisplayName);

        _output.WriteLine($"Retrieved control panel type: {result.DisplayName}");
        _output.WriteLine($"  Name: {result.Name}");
        _output.WriteLine($"  Version: {result.Version}");
        _output.WriteLine($"  Website: {result.WebsiteUrl}");
        _output.WriteLine($"  Active: {result.IsActive}");
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetControlPanelTypeById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetControlPanelTypeById_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task GetControlPanelTypeById_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var controlPanelTypeId = await SeedControlPanelTypes();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Create Control Panel Type Tests

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "3")]
    public async Task CreateControlPanelType_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateControlPanelTypeDto
        {
            Name = "cyberpanel",
            DisplayName = "CyberPanel",
            Description = "CyberPanel - Open-source control panel",
            Version = "2.3",
            WebsiteUrl = "https://cyberpanel.net",
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ControlPanelTypes", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ControlPanelTypeDto>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.DisplayName, result.DisplayName);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(createDto.Version, result.Version);
        Assert.Equal(createDto.WebsiteUrl, result.WebsiteUrl);
        Assert.Equal(createDto.IsActive, result.IsActive);
        Assert.True(result.CreatedAt > DateTime.MinValue);
        Assert.True(result.UpdatedAt > DateTime.MinValue);
        Assert.Equal(result.CreatedAt, result.UpdatedAt, TimeSpan.FromSeconds(1));

        _output.WriteLine($"Created control panel type with ID: {result.Id}");
        _output.WriteLine($"  Name: {result.Name}");
        _output.WriteLine($"  Display Name: {result.DisplayName}");
        _output.WriteLine($"  Version: {result.Version}");
        _output.WriteLine($"  CreatedAt: {result.CreatedAt}");
        _output.WriteLine($"  UpdatedAt: {result.UpdatedAt}");
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task CreateControlPanelType_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateControlPanelTypeDto
        {
            Name = "test",
            DisplayName = "Test Panel",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ControlPanelTypes", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task CreateControlPanelType_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateControlPanelTypeDto
        {
            Name = "test",
            DisplayName = "Test Panel",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ControlPanelTypes", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task CreateControlPanelType_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateControlPanelTypeDto
        {
            Name = "test",
            DisplayName = "Test Panel",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ControlPanelTypes", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Update Control Panel Type Tests

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "4")]
    public async Task UpdateControlPanelType_ValidData_ReturnsOk()
    {
        // Arrange
        var controlPanelTypeId = await SeedControlPanelTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Get the original control panel type to compare timestamps
        var getResponse = await _client.GetAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}");
        var originalType = await getResponse.Content.ReadFromJsonAsync<ControlPanelTypeDto>();
        Assert.NotNull(originalType);

        // Wait a bit to ensure UpdatedAt will be different
        await Task.Delay(100);

        var updateDto = new UpdateControlPanelTypeDto
        {
            Name = "cpanel",
            DisplayName = "cPanel & WHM",
            Description = "Updated cPanel description",
            Version = "11.120",
            WebsiteUrl = "https://cpanel.net",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ControlPanelTypeDto>();
        Assert.NotNull(result);
        Assert.Equal(controlPanelTypeId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.DisplayName, result.DisplayName);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(updateDto.Version, result.Version);
        Assert.Equal(updateDto.WebsiteUrl, result.WebsiteUrl);
        Assert.Equal(updateDto.IsActive, result.IsActive);
        Assert.Equal(originalType.CreatedAt, result.CreatedAt); // CreatedAt should not change
        Assert.True(result.UpdatedAt > originalType.UpdatedAt); // UpdatedAt should be newer

        _output.WriteLine($"Updated control panel type ID {result.Id}:");
        _output.WriteLine($"  New Display Name: {result.DisplayName}");
        _output.WriteLine($"  New Version: {result.Version}");
        _output.WriteLine($"  CreatedAt: {result.CreatedAt} (unchanged)");
        _output.WriteLine($"  UpdatedAt: {result.UpdatedAt} (was: {originalType.UpdatedAt})");
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task UpdateControlPanelType_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateControlPanelTypeDto
        {
            Name = "updated",
            DisplayName = "Updated Panel",
            Description = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/ControlPanelTypes/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task UpdateControlPanelType_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var controlPanelTypeId = await SeedControlPanelTypes();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateControlPanelTypeDto
        {
            Name = "updated",
            DisplayName = "Updated Panel",
            Description = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task UpdateControlPanelType_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var updateDto = new UpdateControlPanelTypeDto
        {
            Name = "updated",
            DisplayName = "Updated Panel",
            Description = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/ControlPanelTypes/1", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Delete Control Panel Type Tests

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "5")]
    public async Task DeleteControlPanelType_ValidId_ReturnsNoContent()
    {
        // Arrange
        var controlPanelTypeId = await SeedControlPanelTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _output.WriteLine($"Successfully deleted control panel type ID: {controlPanelTypeId}");

        // Verify it's actually deleted
        var getResponse = await _client.GetAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task DeleteControlPanelType_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/ControlPanelTypes/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task DeleteControlPanelType_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var controlPanelTypeId = await SeedControlPanelTypes();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/ControlPanelTypes/{controlPanelTypeId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    public async Task DeleteControlPanelType_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/v1/ControlPanelTypes/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "6")]
    public async Task FullCRUDFlow_CreateReadUpdateDelete_Success()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        _output.WriteLine("=== Starting Full CRUD Flow for Control Panel Types ===");

        // Create
        _output.WriteLine("\n1. CREATE:");
        var createDto = new CreateControlPanelTypeDto
        {
            Name = "interworx",
            DisplayName = "InterWorx",
            Description = "InterWorx Control Panel",
            Version = "7.10",
            WebsiteUrl = "https://www.interworx.com",
            IsActive = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/ControlPanelTypes", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ControlPanelTypeDto>();
        Assert.NotNull(created);
        _output.WriteLine($"   Created ID: {created.Id}, Name: {created.DisplayName}");

        // Read
        _output.WriteLine("\n2. READ:");
        var readResponse = await _client.GetAsync($"/api/v1/ControlPanelTypes/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);

        var read = await readResponse.Content.ReadFromJsonAsync<ControlPanelTypeDto>();
        Assert.NotNull(read);
        Assert.Equal(created.Id, read.Id);
        Assert.Equal(createDto.Name, read.Name);
        Assert.Equal(createDto.DisplayName, read.DisplayName);
        _output.WriteLine($"   Retrieved: {read.DisplayName} (v{read.Version})");

        // Update
        _output.WriteLine("\n3. UPDATE:");
        var updateDto = new UpdateControlPanelTypeDto
        {
            Name = "interworx",
            DisplayName = "InterWorx CP",
            Description = "Updated: InterWorx Web Hosting Control Panel",
            Version = "7.11",
            WebsiteUrl = "https://www.interworx.com",
            IsActive = true
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/ControlPanelTypes/{created.Id}", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<ControlPanelTypeDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateDto.DisplayName, updated.DisplayName);
        Assert.Equal(updateDto.Version, updated.Version);
        _output.WriteLine($"   Updated to: {updated.DisplayName} (v{updated.Version})");

        // Delete
        _output.WriteLine("\n4. DELETE:");
        var deleteResponse = await _client.DeleteAsync($"/api/v1/ControlPanelTypes/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        _output.WriteLine($"   Deleted ID: {created.Id}");

        // Verify deletion
        _output.WriteLine("\n5. VERIFY DELETION:");
        var verifyResponse = await _client.GetAsync($"/api/v1/ControlPanelTypes/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, verifyResponse.StatusCode);
        _output.WriteLine("   Confirmed: Resource no longer exists");

        _output.WriteLine("\n=== Full CRUD Flow Completed Successfully ===");
    }

    [Fact]
    [Trait("Category", "ControlPanelTypes")]
    [Trait("Priority", "7")]
    public async Task ActiveFilter_OnlyReturnsActiveTypes()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create active control panel type
        var activeDto = new CreateControlPanelTypeDto
        {
            Name = "cpanel",
            DisplayName = "cPanel",
            Description = "Active control panel",
            IsActive = true
        };
        await _client.PostAsJsonAsync("/api/v1/ControlPanelTypes", activeDto);

        // Create inactive control panel type
        var inactiveDto = new CreateControlPanelTypeDto
        {
            Name = "discontinued",
            DisplayName = "Discontinued Panel",
            Description = "Inactive control panel",
            IsActive = false
        };
        await _client.PostAsJsonAsync("/api/v1/ControlPanelTypes", inactiveDto);

        // Act
        var response = await _client.GetAsync("/api/v1/ControlPanelTypes/active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ControlPanelTypeDto>>();
        Assert.NotNull(result);
        Assert.All(result, type => Assert.True(type.IsActive));
        Assert.DoesNotContain(result, type => type.Name == "discontinued");

        _output.WriteLine($"Active filter returned {result.Count()} active control panel types");
        _output.WriteLine("Verified that inactive types are excluded");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Seeds test control panel types in the database
    /// Returns the ID of the first seeded control panel type
    /// </summary>
    private async Task<int> SeedControlPanelTypes()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clear existing control panel types to avoid UNIQUE constraint violations
        var existingControlPanelTypes = await context.ControlPanelTypes.ToListAsync();
        if (existingControlPanelTypes.Any())
        {
            context.ControlPanelTypes.RemoveRange(existingControlPanelTypes);
            await context.SaveChangesAsync();
        }

        var now = DateTime.UtcNow;
        var controlPanelTypes = new[]
        {
            new ControlPanelType
            {
                Name = "cpanel",
                DisplayName = "cPanel",
                Description = "cPanel & WHM - Industry standard control panel",
                Version = "11.110",
                WebsiteUrl = "https://cpanel.net",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new ControlPanelType
            {
                Name = "plesk",
                DisplayName = "Plesk",
                Description = "Plesk Obsidian - Modern hosting platform",
                Version = "18.0",
                WebsiteUrl = "https://www.plesk.com",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new ControlPanelType
            {
                Name = "directadmin",
                DisplayName = "DirectAdmin",
                Description = "DirectAdmin - Lightweight control panel",
                Version = "1.65",
                WebsiteUrl = "https://www.directadmin.com",
                IsActive = false,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.ControlPanelTypes.AddRangeAsync(controlPanelTypes);
        await context.SaveChangesAsync();

        return controlPanelTypes[0].Id;
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
