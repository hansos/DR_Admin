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
public class DnsZonePackagesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public DnsZonePackagesControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All DNS Zone Packages Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    [Trait("Priority", "1")]
    public async Task GetAllDnsZonePackages_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsZonePackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsZonePackageDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} DNS zone packages");
        foreach (var package in result)
        {
            Console.WriteLine($"  - {package.Name}: {(package.IsDefault ? "DEFAULT" : "")}{(package.IsActive ? "Active" : "Inactive")}");
        }
    }

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    public async Task GetAllDnsZonePackages_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsZonePackages();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    public async Task GetAllDnsZonePackages_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsZonePackages();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    public async Task GetAllDnsZonePackages_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get All With Records Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    [Trait("Priority", "2")]
    public async Task GetAllDnsZonePackagesWithRecords_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsZonePackagesWithRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages/with-records", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsZonePackageDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        
        var packageWithRecords = result.FirstOrDefault(p => p.Records.Any());
        Assert.NotNull(packageWithRecords);
        Assert.NotEmpty(packageWithRecords.Records);

        Console.WriteLine($"Retrieved {result.Count()} DNS zone packages with records");
        foreach (var package in result)
        {
            Console.WriteLine($"  - {package.Name}: {package.Records.Count} records");
        }
    }

    #endregion

    #region Get Active Packages Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    public async Task GetActiveDnsZonePackages_ReturnsOnlyActive()
    {
        // Arrange
        await SeedDnsZonePackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsZonePackageDto>>(TestContext.Current.CancellationToken );
        Assert.NotNull(result);
        Assert.All(result, package => Assert.True(package.IsActive));

        Console.WriteLine($"Retrieved {result.Count()} active DNS zone packages");
    }

    #endregion

    #region Get Default Package Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    [Trait("Priority", "3")]
    public async Task GetDefaultDnsZonePackage_ReturnsDefault()
    {
        // Arrange
        await SeedDnsZonePackagesWithRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages/default", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageDto>();
        Assert.NotNull(result);
        Assert.True(result.IsDefault);
        Assert.NotEmpty(result.Records);

        Console.WriteLine($"Default package: {result.Name} with {result.Records.Count} records");
    }

    #endregion

    #region Get By Id Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    public async Task GetDnsZonePackageById_ValidId_ReturnsOk()
    {
        // Arrange
        var packageId = await SeedDnsZonePackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsZonePackages/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageDto>();
        Assert.NotNull(result);
        Assert.Equal(packageId, result.Id);

        Console.WriteLine($"Retrieved package: {result.Name}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    public async Task GetDnsZonePackageById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsZonePackages/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Create Package Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    [Trait("Priority", "4")]
    public async Task CreateDnsZonePackage_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsZonePackageDto
        {
            Name = "Test Package",
            Description = "Test DNS zone package",
            IsActive = true,
            IsDefault = false,
            SortOrder = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsZonePackages", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageDto>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(createDto.IsActive, result.IsActive);
        Assert.Equal(createDto.IsDefault, result.IsDefault);

        Console.WriteLine($"Created DNS zone package with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    public async Task CreateDnsZonePackage_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsZonePackageDto
        {
            Name = "Test Package",
            Description = "Test",
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsZonePackages", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update Package Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    [Trait("Priority", "5")]
    public async Task UpdateDnsZonePackage_ValidData_ReturnsOk()
    {
        // Arrange
        var packageId = await SeedDnsZonePackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsZonePackageDto
        {
            Name = "Updated Package Name",
            Description = "Updated description",
            IsActive = true,
            IsDefault = false,
            SortOrder = 20
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsZonePackages/{packageId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsZonePackageDto>();
        Assert.NotNull(result);
        Assert.Equal(packageId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);

        Console.WriteLine($"Updated package ID: {result.Id}");
    }

    #endregion

    #region Delete Package Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    [Trait("Priority", "6")]
    public async Task DeleteDnsZonePackage_ValidId_ReturnsNoContent()
    {
        // Arrange
        var packageId = await SeedDnsZonePackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsZonePackages/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted DNS zone package ID: {packageId}");
    }

    #endregion

    #region Apply Package To Domain Tests

    [Fact]
    [Trait("Category", "DnsZonePackages")]
    [Trait("Priority", "7")]
    public async Task ApplyPackageToDomain_ValidIds_ReturnsOk()
    {
        // Arrange
        var (packageId, domainId) = await SeedPackageAndDomain();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/v1/DnsZonePackages/{packageId}/apply-to-domain/{domainId}", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Console.WriteLine($"Applied package {packageId} to domain {domainId}");

        // Verify DNS records were created
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dnsRecords = await context.DnsRecords.Where(r => r.DomainId == domainId).ToListAsync();
        Assert.NotEmpty(dnsRecords);

        Console.WriteLine($"Created {dnsRecords.Count} DNS records");
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedDnsZonePackages()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean up existing test data
        var existingPackages = await context.DnsZonePackages.ToListAsync();
        context.DnsZonePackages.RemoveRange(existingPackages);
        await context.SaveChangesAsync();

        var package = new DnsZonePackage
        {
            Name = "Basic Web Hosting",
            Description = "Basic DNS records for web hosting",
            IsActive = true,
            IsDefault = true,
            SortOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsZonePackages.Add(package);
        await context.SaveChangesAsync();

        return package.Id;
    }

    private async Task<int> SeedDnsZonePackagesWithRecords()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean up
        var existingRecords = await context.DnsZonePackageRecords.ToListAsync();
        context.DnsZonePackageRecords.RemoveRange(existingRecords);
        var existingPackages = await context.DnsZonePackages.ToListAsync();
        context.DnsZonePackages.RemoveRange(existingPackages);
        await context.SaveChangesAsync();

        // Ensure DNS record types exist
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

        var package = new DnsZonePackage
        {
            Name = "Complete DNS Setup",
            Description = "Complete DNS records for web and email",
            IsActive = true,
            IsDefault = true,
            SortOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsZonePackages.Add(package);
        await context.SaveChangesAsync();

        // Add DNS zone package record
        var record = new DnsZonePackageRecord
        {
            DnsZonePackageId = package.Id,
            DnsRecordTypeId = aRecordType.Id,
            Name = "@",
            Value = "192.0.2.1",
            TTL = 3600,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsZonePackageRecords.Add(record);
        await context.SaveChangesAsync();

        return package.Id;
    }

    private async Task<(int packageId, int domainId)> SeedPackageAndDomain()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create package with records
        var packageId = await SeedDnsZonePackagesWithRecords();

        // Create a test domain
        var customer = await context.Customers.FirstOrDefaultAsync();
        if (customer == null)
        {
            customer = new Customer
            {
                Name = "Test Customer",
                Email = "test@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
        }

        var registrar = await context.Registrars.FirstOrDefaultAsync();
        if (registrar == null)
        {
            registrar = new Registrar
            {
                Name = "Test Registrar",
                Code = "TEST",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Registrars.Add(registrar);
            await context.SaveChangesAsync();
        }

        var domain = new Domain
        {
            CustomerId = customer.Id,
            RegistrarId = registrar.Id,
            Name = "test-apply.com",
            Status = "Active",
            RegistrationDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            AutoRenew = false,
            PrivacyProtection = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Domains.Add(domain);
        await context.SaveChangesAsync();

        return (packageId, domain.Id);
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

