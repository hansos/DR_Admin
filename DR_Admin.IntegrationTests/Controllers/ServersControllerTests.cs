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
public class ServersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public ServersControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Servers Tests

    [Fact]
    [Trait("Category", "Servers")]
    [Trait("Priority", "1")]
    public async Task GetAllServers_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedServers();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Servers", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ServerDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} servers");
        foreach (var server in result)
        {
            Console.WriteLine($"  - {server.Name} ({server.ServerType}): {server.Status}");
        }
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task GetAllServers_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedServers();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Servers", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task GetAllServers_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Servers", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task GetAllServers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Servers", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Server By Id Tests

    [Fact]
    [Trait("Category", "Servers")]
    [Trait("Priority", "2")]
    public async Task GetServerById_ValidId_ReturnsOk()
    {
        // Arrange
        var serverId = await SeedServers();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Servers/{serverId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ServerDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(serverId, result.Id);
        Assert.NotEmpty(result.Name);

        Console.WriteLine($"Retrieved server: {result.Name}");
        Console.WriteLine($"  Type: {result.ServerType}");
        Console.WriteLine($"  OS: {result.OperatingSystem}");
        Console.WriteLine($"  Status: {result.Status}");
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task GetServerById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Servers/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Create Server Tests

    [Fact]
    [Trait("Category", "Servers")]
    [Trait("Priority", "3")]
    public async Task CreateServer_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateServerDto
        {
            Name = "Test Server 1",
            ServerType = "Cloud",
            HostProvider = "AWS",
            Location = "US-East",
            OperatingSystem = "Ubuntu 22.04 LTS",
            Status = "Active",
            CpuCores = 8,
            RamMB = 16384,
            DiskSpaceGB = 500,
            Notes = "Test server for integration tests"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Servers", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ServerDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.ServerType, result.ServerType);
        Assert.Equal(createDto.HostProvider, result.HostProvider);
        Assert.Equal(createDto.Location, result.Location);
        Assert.Equal(createDto.OperatingSystem, result.OperatingSystem);
        Assert.Equal(createDto.Status, result.Status);
        Assert.Equal(createDto.CpuCores, result.CpuCores);
        Assert.Equal(createDto.RamMB, result.RamMB);
        Assert.Equal(createDto.DiskSpaceGB, result.DiskSpaceGB);

        Console.WriteLine($"Created server with ID: {result.Id}");
        Console.WriteLine($"  Name: {result.Name}");
        Console.WriteLine($"  Type: {result.ServerType}");
        Console.WriteLine($"  Specs: {result.CpuCores} cores, {result.RamMB}MB RAM, {result.DiskSpaceGB}GB disk");
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task CreateServer_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateServerDto
        {
            Name = "Test Server",
            ServerType = "Physical",
            OperatingSystem = "Windows Server 2022",
            Status = "Active"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Servers", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Update Server Tests

    [Fact]
    [Trait("Category", "Servers")]
    [Trait("Priority", "4")]
    public async Task UpdateServer_ValidData_ReturnsOk()
    {
        // Arrange
        var serverId = await SeedServers();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateServerDto
        {
            Name = "Updated Server Name",
            ServerType = "Virtual",
            HostProvider = "DigitalOcean",
            Location = "EU-West",
            OperatingSystem = "CentOS 8",
            Status = "Maintenance",
            CpuCores = 16,
            RamMB = 32768,
            DiskSpaceGB = 1000,
            Notes = "Updated notes"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Servers/{serverId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ServerDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(serverId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Status, result.Status);
        Assert.Equal(updateDto.CpuCores, result.CpuCores);

        Console.WriteLine($"Updated server ID: {result.Id}");
        Console.WriteLine($"  New name: {result.Name}");
        Console.WriteLine($"  New status: {result.Status}");
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task UpdateServer_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateServerDto
        {
            Name = "Test",
            ServerType = "Cloud",
            OperatingSystem = "Linux",
            Status = "Active"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/Servers/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Delete Server Tests

    [Fact]
    [Trait("Category", "Servers")]
    [Trait("Priority", "5")]
    public async Task DeleteServer_ValidId_ReturnsNoContent()
    {
        // Arrange
        var serverId = await SeedServers();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Servers/{serverId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted server ID: {serverId}");
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task DeleteServer_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/Servers/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Servers")]
    public async Task DeleteServer_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var serverId = await SeedServers();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Servers/{serverId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedServers()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean up existing test data
        var existingServers = await context.Servers.ToListAsync();
        context.Servers.RemoveRange(existingServers);
        await context.SaveChangesAsync();

        var server = new Server
        {
            Name = "Primary Web Server",
            ServerType = "Physical",
            HostProvider = "On-Premise",
            Location = "DataCenter-1",
            OperatingSystem = "Ubuntu 22.04 LTS",
            Status = "Active",
            CpuCores = 32,
            RamMB = 65536,
            DiskSpaceGB = 2000,
            Notes = "Main production server",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Servers.Add(server);
        await context.SaveChangesAsync();

        return server.Id;
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

