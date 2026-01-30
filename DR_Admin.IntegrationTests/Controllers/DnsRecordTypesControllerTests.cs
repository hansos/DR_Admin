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
public class DnsRecordTypesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public DnsRecordTypesControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All DNS Record Types Tests

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    [Trait("Priority", "1")]
    public async Task GetAllDnsRecordTypes_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsRecordTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsRecordTypeDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} DNS record types");
        foreach (var recordType in result)
        {
            Console.WriteLine($"  - {recordType.Type}: {recordType.Description} (Active: {recordType.IsActive})");
        }
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetAllDnsRecordTypes_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsRecordTypes();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetAllDnsRecordTypes_WithCustomerRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsRecordTypes();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetAllDnsRecordTypes_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Active DNS Record Types Tests

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    [Trait("Priority", "2")]
    public async Task GetActiveDnsRecordTypes_ReturnsOnlyActive()
    {
        // Arrange
        await SeedMixedDnsRecordTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsRecordTypeDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.All(result, recordType => Assert.True(recordType.IsActive));

        Console.WriteLine($"Retrieved {result.Count()} active DNS record types");
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetActiveDnsRecordTypes_WithCustomerRole_ReturnsOk()
    {
        // Arrange
        await SeedMixedDnsRecordTypes();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get DNS Record Type By Id Tests

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    [Trait("Priority", "3")]
    public async Task GetDnsRecordTypeById_ValidId_ReturnsOk()
    {
        // Arrange
        var recordTypeId = await SeedDnsRecordTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsRecordTypes/{recordTypeId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordTypeDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(recordTypeId, result.Id);

        Console.WriteLine($"Retrieved DNS record type: {result.Type}");
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetDnsRecordTypeById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetDnsRecordTypeById_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var recordTypeId = await SeedDnsRecordTypes();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsRecordTypes/{recordTypeId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Get DNS Record Type By Type Tests

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    [Trait("Priority", "4")]
    public async Task GetDnsRecordTypeByType_ValidType_ReturnsOk()
    {
        // Arrange
        await SeedDnsRecordTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes/type/A", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordTypeDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal("A", result.Type);

        Console.WriteLine($"Retrieved DNS record type: {result.Type} - {result.Description}");
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetDnsRecordTypeByType_InvalidType_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes/type/INVALID", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task GetDnsRecordTypeByType_WithCustomerRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsRecordTypes();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecordTypes/type/A", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Create DNS Record Type Tests

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    [Trait("Priority", "5")]
    public async Task CreateDnsRecordType_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var timestamp = DateTime.UtcNow.Ticks;
        var createDto = new CreateDnsRecordTypeDto
        {
            Type = $"TEST{timestamp}",
            Description = "Test record type",
            HasPriority = false,
            HasWeight = false,
            HasPort = false,
            IsEditableByUser = true,
            IsActive = true,
            DefaultTTL = 3600
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsRecordTypes", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordTypeDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Type, result.Type);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(createDto.DefaultTTL, result.DefaultTTL);

        Console.WriteLine($"Created DNS record type with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task CreateDnsRecordType_WithMxTypeFeatures_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var timestamp = DateTime.UtcNow.Ticks;
        var createDto = new CreateDnsRecordTypeDto
        {
            Type = $"TESTMX{timestamp}",
            Description = "Test MX-like record type",
            HasPriority = true,
            HasWeight = false,
            HasPort = false,
            IsEditableByUser = true,
            IsActive = true,
            DefaultTTL = 3600
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsRecordTypes", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordTypeDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.HasPriority);

        Console.WriteLine($"Created MX-like DNS record type with priority support");
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task CreateDnsRecordType_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsRecordTypeDto
        {
            Type = "TEST2",
            Description = "Test",
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsRecordTypes", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update DNS Record Type Tests

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    [Trait("Priority", "6")]
    public async Task UpdateDnsRecordType_ValidData_ReturnsOk()
    {
        // Arrange
        var recordTypeId = await SeedDnsRecordTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordTypeDto
        {
            Type = "A",
            Description = "Updated IPv4 address record",
            HasPriority = false,
            HasWeight = false,
            HasPort = false,
            IsEditableByUser = true,
            IsActive = true,
            DefaultTTL = 7200
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsRecordTypes/{recordTypeId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordTypeDto>(TestContext.Current.CancellationToken);      
        Assert.NotNull(result);
        Assert.Equal(recordTypeId, result.Id);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(updateDto.DefaultTTL, result.DefaultTTL);

        Console.WriteLine($"Updated DNS record type ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task UpdateDnsRecordType_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordTypeDto
        {
            Type = "A",
            Description = "Test",
            IsActive = true,
            DefaultTTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/DnsRecordTypes/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task UpdateDnsRecordType_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var recordTypeId = await SeedDnsRecordTypes();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordTypeDto
        {
            Type = "A",
            Description = "Test",
            IsActive = true,
            DefaultTTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsRecordTypes/{recordTypeId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Delete DNS Record Type Tests

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    [Trait("Priority", "7")]
    public async Task DeleteDnsRecordType_ValidId_ReturnsNoContent()
    {
        // Arrange
        var recordTypeId = await SeedDeletableDnsRecordType();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsRecordTypes/{recordTypeId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted DNS record type ID: {recordTypeId}");
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task DeleteDnsRecordType_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/DnsRecordTypes/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecordTypes")]
    public async Task DeleteDnsRecordType_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var recordTypeId = await SeedDeletableDnsRecordType();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsRecordTypes/{recordTypeId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedDnsRecordTypes()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Check if A record type already exists
        var aRecordType = await context.DnsRecordTypes.FirstOrDefaultAsync(t => t.Type == "A");
        if (aRecordType != null)
        {
            return aRecordType.Id;
        }

        // Create A record type
        aRecordType = new DnsRecordType
        {
            Type = "A",
            Description = "IPv4 address record",
            HasPriority = false,
            HasWeight = false,
            HasPort = false,
            IsEditableByUser = true,
            IsActive = true,
            DefaultTTL = 3600,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsRecordTypes.Add(aRecordType);
        await context.SaveChangesAsync();

        return aRecordType.Id;
    }

    private async Task SeedMixedDnsRecordTypes()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean up existing test data for mixed types
        var existingTypes = await context.DnsRecordTypes
            .Where(t => t.Type.StartsWith("TEST"))
            .ToListAsync();
        context.DnsRecordTypes.RemoveRange(existingTypes);
        await context.SaveChangesAsync();

        var timestamp = DateTime.UtcNow.Ticks;
        var recordTypes = new List<DnsRecordType>
        {
            new DnsRecordType
            {
                Type = $"TESTACTIVE{timestamp}",
                Description = "Active test record type",
                HasPriority = false,
                HasWeight = false,
                HasPort = false,
                IsEditableByUser = true,
                IsActive = true,
                DefaultTTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new DnsRecordType
            {
                Type = $"TESTINACTIVE{timestamp}",
                Description = "Inactive test record type",
                HasPriority = false,
                HasWeight = false,
                HasPort = false,
                IsEditableByUser = true,
                IsActive = false,
                DefaultTTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.DnsRecordTypes.AddRange(recordTypes);
        await context.SaveChangesAsync();
    }

    private async Task<int> SeedDeletableDnsRecordType()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var recordType = new DnsRecordType
        {
            Type = $"TESTDELETE{timestamp}",
            Description = "Deletable test record type",
            HasPriority = false,
            HasWeight = false,
            HasPort = false,
            IsEditableByUser = true,
            IsActive = true,
            DefaultTTL = 3600,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsRecordTypes.Add(recordType);
        await context.SaveChangesAsync();

        return recordType.Id;
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

        Console.WriteLine($"Created {roleName} user: {username}");

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

