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
public class HostingPackagesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public HostingPackagesControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Hosting Packages Tests

    [Fact]
    [Trait("Category", "HostingPackages")]
    [Trait("Priority", "1")]
    public async Task GetAllHostingPackages_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedHostingPackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/HostingPackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<HostingPackageDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} hosting packages");
        foreach (var package in result)
        {
            Console.WriteLine($"  - {package.Name}: ${package.MonthlyPrice}/month (Active: {package.IsActive})");
        }
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task GetAllHostingPackages_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedHostingPackages();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/HostingPackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task GetAllHostingPackages_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedHostingPackages();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/HostingPackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task GetAllHostingPackages_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/HostingPackages", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Active Hosting Packages Tests

    [Fact]
    [Trait("Category", "HostingPackages")]
    [Trait("Priority", "2")]
    public async Task GetActiveHostingPackages_ReturnsOnlyActive()
    {
        // Arrange
        await SeedMixedHostingPackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/HostingPackages/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<HostingPackageDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.All(result, package => Assert.True(package.IsActive));

        Console.WriteLine($"Retrieved {result.Count()} active hosting packages");
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task GetActiveHostingPackages_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedMixedHostingPackages();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/HostingPackages/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get Hosting Package By Id Tests

    [Fact]
    [Trait("Category", "HostingPackages")]
    [Trait("Priority", "3")]
    public async Task GetHostingPackageById_ValidId_ReturnsOk()
    {
        // Arrange
        var packageId = await SeedHostingPackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/HostingPackages/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<HostingPackageDto>();
        Assert.NotNull(result);
        Assert.Equal(packageId, result.Id);

        Console.WriteLine($"Retrieved hosting package: {result.Name} - ${result.MonthlyPrice}");
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task GetHostingPackageById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/HostingPackages/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task GetHostingPackageById_WithSupportRole_ReturnsOk()
    {
        // Arrange
        var packageId = await SeedHostingPackages();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/HostingPackages/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Create Hosting Package Tests

    [Fact]
    [Trait("Category", "HostingPackages")]
    [Trait("Priority", "4")]
    public async Task CreateHostingPackage_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateHostingPackageDto
        {
            Name = "Premium Hosting",
            Description = "Premium hosting package with advanced features",
            MonthlyPrice = 49.99m,
            YearlyPrice = 499.99m,
            DiskSpaceMB = 102400,
            BandwidthMB = 1024000,
            EmailAccounts = 50,
            Databases = 10,
            Domains = 10,
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/HostingPackages", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<HostingPackageDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.MonthlyPrice, result.MonthlyPrice);
        Assert.Equal(createDto.DiskSpaceMB, result.DiskSpaceMB);

        Console.WriteLine($"Created hosting package with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task CreateHostingPackage_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateHostingPackageDto
        {
            Name = "Test Package",
            MonthlyPrice = 9.99m,
            YearlyPrice = 99.99m,
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/HostingPackages", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update Hosting Package Tests

    [Fact]
    [Trait("Category", "HostingPackages")]
    [Trait("Priority", "5")]
    public async Task UpdateHostingPackage_ValidData_ReturnsOk()
    {
        // Arrange
        var packageId = await SeedHostingPackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateHostingPackageDto
        {
            Name = "Updated Package",
            Description = "Updated description",
            MonthlyPrice = 59.99m,
            YearlyPrice = 599.99m,
            DiskSpaceMB = 153600,
            BandwidthMB = 1536000,
            EmailAccounts = 100,
            Databases = 20,
            Domains = 15,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/HostingPackages/{packageId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<HostingPackageDto>();
        Assert.NotNull(result);
        Assert.Equal(packageId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.MonthlyPrice, result.MonthlyPrice);

        Console.WriteLine($"Updated hosting package ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task UpdateHostingPackage_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateHostingPackageDto
        {
            Name = "Test",
            MonthlyPrice = 9.99m,
            YearlyPrice = 99.99m,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/HostingPackages/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task UpdateHostingPackage_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var packageId = await SeedHostingPackages();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateHostingPackageDto
        {
            Name = "Test",
            MonthlyPrice = 9.99m,
            YearlyPrice = 99.99m,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/HostingPackages/{packageId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Delete Hosting Package Tests

    [Fact]
    [Trait("Category", "HostingPackages")]
    [Trait("Priority", "6")]
    public async Task DeleteHostingPackage_ValidId_ReturnsNoContent()
    {
        // Arrange
        var packageId = await SeedHostingPackages();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/HostingPackages/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted hosting package ID: {packageId}");
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task DeleteHostingPackage_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/HostingPackages/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "HostingPackages")]
    public async Task DeleteHostingPackage_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var packageId = await SeedHostingPackages();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/HostingPackages/{packageId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedHostingPackages()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var package = new HostingPackage
        {
            Name = $"Basic Hosting {timestamp}",
            Description = "Basic hosting package",
            MonthlyPrice = 9.99m,
            YearlyPrice = 99.99m,
            DiskSpaceMB = 10240,
            BandwidthMB = 102400,
            EmailAccounts = 5,
            Databases = 1,
            Domains = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.HostingPackages.Add(package);
        await context.SaveChangesAsync();

        return package.Id;
    }

    private async Task SeedMixedHostingPackages()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timestamp = DateTime.UtcNow.Ticks;
        var packages = new List<HostingPackage>
        {
            new HostingPackage
            {
                Name = $"Active Package {timestamp}",
                Description = "Active hosting package",
                MonthlyPrice = 19.99m,
                YearlyPrice = 199.99m,
                DiskSpaceMB = 20480,
                BandwidthMB = 204800,
                EmailAccounts = 10,
                Databases = 2,
                Domains = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new HostingPackage
            {
                Name = $"Inactive Package {timestamp}",
                Description = "Inactive hosting package",
                MonthlyPrice = 29.99m,
                YearlyPrice = 299.99m,
                DiskSpaceMB = 30720,
                BandwidthMB = 307200,
                EmailAccounts = 15,
                Databases = 3,
                Domains = 3,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.HostingPackages.AddRange(packages);
        await context.SaveChangesAsync();
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

