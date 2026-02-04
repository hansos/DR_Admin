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
public class DnsZonePackageRecordsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public DnsZonePackageRecordsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All DNS Zone Package Records Tests

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    [Trait("Priority", "1")]
    public async Task GetAllDnsZonePackageRecords_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsZonePackageRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackageRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsZonePackageRecordDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} DNS zone package records");
        foreach (var record in result)
        {
            Console.WriteLine($"  - {record.Name}: {record.Value} (TTL: {record.TTL})");
        }
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task GetAllDnsZonePackageRecords_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsZonePackageRecords();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackageRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task GetAllDnsZonePackageRecords_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        await SeedDnsZonePackageRecords();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackageRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task GetAllDnsZonePackageRecords_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackageRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Records By Package Id Tests

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    [Trait("Priority", "2")]
    public async Task GetRecordsByPackageId_ValidPackageId_ReturnsOk()
    {
        // Arrange
        var (packageId, _) = await SeedPackageWithRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsZonePackageRecords/package/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsZonePackageRecordDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, record => Assert.Equal(packageId, record.DnsZonePackageId));

        Console.WriteLine($"Retrieved {result.Count()} records for package {packageId}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task GetRecordsByPackageId_EmptyPackage_ReturnsEmptyList()
    {
        // Arrange
        var packageId = await SeedEmptyPackage();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsZonePackageRecords/package/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsZonePackageRecordDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task GetRecordsByPackageId_WithSalesRole_ReturnsOk()
    {
        // Arrange
        var (packageId, _) = await SeedPackageWithRecords();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsZonePackageRecords/package/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get DNS Zone Package Record By Id Tests

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    [Trait("Priority", "3")]
    public async Task GetDnsZonePackageRecordById_ValidId_ReturnsOk()
    {
        // Arrange
        var (_, recordId) = await SeedPackageWithRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsZonePackageRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(recordId, result.Id);

        Console.WriteLine($"Retrieved DNS zone package record: {result.Name} = {result.Value}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task GetDnsZonePackageRecordById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackageRecords/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task GetDnsZonePackageRecordById_WithSupportRole_ReturnsOk()
    {
        // Arrange
        var (_, recordId) = await SeedPackageWithRecords();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsZonePackageRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Create DNS Zone Package Record Tests

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    [Trait("Priority", "4")]
    public async Task CreateDnsZonePackageRecord_ValidData_ReturnsCreated()
    {
        // Arrange
        var (packageId, recordTypeId) = await SeedPackageAndRecordType();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsZonePackageRecordDto
        {
            DnsZonePackageId = packageId,
            DnsRecordTypeId = recordTypeId,
            Name = "www",
            Value = "192.0.2.100",
            TTL = 3600,
            Notes = "Test web server record"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsZonePackageRecords", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.DnsZonePackageId, result.DnsZonePackageId);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Value, result.Value);

        Console.WriteLine($"Created DNS zone package record with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task CreateDnsZonePackageRecord_WithMxRecord_IncludesPriority()
    {
        // Arrange
        var (packageId, mxRecordTypeId) = await SeedPackageAndMxRecordType();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsZonePackageRecordDto
        {
            DnsZonePackageId = packageId,
            DnsRecordTypeId = mxRecordTypeId,
            Name = "@",
            Value = "mail.example.com",
            TTL = 3600,
            Priority = 10,
            Notes = "Primary mail server"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsZonePackageRecords", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(10, result.Priority);

        Console.WriteLine($"Created MX record with priority {result.Priority}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task CreateDnsZonePackageRecord_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var (packageId, recordTypeId) = await SeedPackageAndRecordType();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsZonePackageRecordDto
        {
            DnsZonePackageId = packageId,
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.50",
            TTL = 3600
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsZonePackageRecords", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update DNS Zone Package Record Tests

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    [Trait("Priority", "5")]
    public async Task UpdateDnsZonePackageRecord_ValidData_ReturnsOk()
    {
        // Arrange
        var (packageId, recordId, recordTypeId) = await SeedPackageRecordForUpdate();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsZonePackageRecordDto
        {
            DnsRecordTypeId = recordTypeId,
            Name = "updated",
            Value = "192.0.2.200",
            TTL = 7200,
            Notes = "Updated record"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsZonePackageRecords/{recordId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(recordId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Value, result.Value);
        Assert.Equal(updateDto.TTL, result.TTL);

        Console.WriteLine($"Updated DNS zone package record ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task UpdateDnsZonePackageRecord_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var (_, _, recordTypeId) = await SeedPackageRecordForUpdate();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsZonePackageRecordDto
        {
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.1",
            TTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/DnsZonePackageRecords/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task UpdateDnsZonePackageRecord_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var (packageId, recordId, recordTypeId) = await SeedPackageRecordForUpdate();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsZonePackageRecordDto
        {
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.1",
            TTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsZonePackageRecords/{recordId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Delete DNS Zone Package Record Tests

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    [Trait("Priority", "6")]
    public async Task DeleteDnsZonePackageRecord_ValidId_ReturnsNoContent()
    {
        // Arrange
        var (_, recordId) = await SeedPackageWithRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsZonePackageRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted DNS zone package record ID: {recordId}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task DeleteDnsZonePackageRecord_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/DnsZonePackageRecords/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsZonePackageRecords")]
    public async Task DeleteDnsZonePackageRecord_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var (_, recordId) = await SeedPackageWithRecords();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsZonePackageRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedDnsZonePackageRecords()
    {
        var (_, recordId) = await SeedPackageWithRecords();
        return recordId;
    }

    private async Task<(int packageId, int recordId)> SeedPackageWithRecords()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (packageId, recordTypeId) = await SeedPackageAndRecordType();

        var record = new DnsZonePackageRecord
        {
            DnsZonePackageId = packageId,
            DnsRecordTypeId = recordTypeId,
            Name = "@",
            Value = "192.0.2.1",
            TTL = 3600,
            Notes = "Root domain record",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsZonePackageRecords.Add(record);
        await context.SaveChangesAsync();

        return (packageId, record.Id);
    }

    private async Task<int> SeedEmptyPackage()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var package = new DnsZonePackage
        {
            Name = $"Empty Package {timestamp}",
            Description = "Package with no records",
            IsActive = true,
            IsDefault = false,
            SortOrder = 99,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsZonePackages.Add(package);
        await context.SaveChangesAsync();

        return package.Id;
    }

    private async Task<(int packageId, int recordTypeId)> SeedPackageAndRecordType()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure A record type exists
        var aRecordType = await context.DnsRecordTypes.FirstOrDefaultAsync(t => t.Type == "A");
        if (aRecordType == null)
        {
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
        }

        // Create package
        var timestamp = DateTime.UtcNow.Ticks;
        var package = new DnsZonePackage
        {
            Name = $"Test Package {timestamp}",
            Description = "Test DNS zone package",
            IsActive = true,
            IsDefault = false,
            SortOrder = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsZonePackages.Add(package);
        await context.SaveChangesAsync();

        return (package.Id, aRecordType.Id);
    }

    private async Task<(int packageId, int mxRecordTypeId)> SeedPackageAndMxRecordType()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure MX record type exists
        var mxRecordType = await context.DnsRecordTypes.FirstOrDefaultAsync(t => t.Type == "MX");
        if (mxRecordType == null)
        {
            mxRecordType = new DnsRecordType
            {
                Type = "MX",
                Description = "Mail exchange record",
                HasPriority = true,
                HasWeight = false,
                HasPort = false,
                IsEditableByUser = true,
                IsActive = true,
                DefaultTTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.DnsRecordTypes.Add(mxRecordType);
            await context.SaveChangesAsync();
        }

        var (packageId, _) = await SeedPackageAndRecordType();
        return (packageId, mxRecordType.Id);
    }

    private async Task<(int packageId, int recordId, int recordTypeId)> SeedPackageRecordForUpdate()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (packageId, recordTypeId) = await SeedPackageAndRecordType();

        var record = new DnsZonePackageRecord
        {
            DnsZonePackageId = packageId,
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.100",
            TTL = 3600,
            Notes = "Test record for update",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsZonePackageRecords.Add(record);
        await context.SaveChangesAsync();

        return (packageId, record.Id, recordTypeId);
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

