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
public class UsersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly TestWebApplicationFactory _factory;

    public UsersControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
    }

    #region Get All Users Tests

    [Fact]
    [Trait("Category", "Users")]
    [Trait("Priority", "1")]
    public async Task GetAllUsers_WithAdminRole_ReturnsOk()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        _output.WriteLine($"Retrieved {result.Count()} users");
    }

    [Fact]
    [Trait("Category", "Users")]
    public async Task GetAllUsers_WithSupportRole_ReturnsOk()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Users")]
    public async Task GetAllUsers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Users")]
    public async Task GetAllUsers_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Users");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Get User By Id Tests

    [Fact]
    [Trait("Category", "Users")]
    [Trait("Priority", "2")]
    public async Task GetUserById_ValidId_ReturnsOk()
    {
        // Arrange
        var userId = await SeedUser();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);

        _output.WriteLine($"Retrieved user: {result.Username}");
    }

    [Fact]
    [Trait("Category", "Users")]
    public async Task GetUserById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Users/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedUser()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customer = new Customer
        {
            Name = "Test Customer",
            Email = $"testuser{DateTime.UtcNow.Ticks}@example.com",
            Phone = "555-USER",
            Address = "123 User St",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        var user = new User
        {
            CustomerId = customer.Id,
            Username = $"testuser{DateTime.UtcNow.Ticks}",
            Email = customer.Email,
            PasswordHash = "Test@1234",
            EmailConfirmed = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user.Id;
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Admin");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<string> GetSupportTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Support");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<string> GetCustomerTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Customer");
        return await LoginAndGetTokenAsync(username);
    }

    private async Task<(string username, string email)> CreateUserWithRole(string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            role = new Role { Name = roleName, Description = $"{roleName} role" };
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

        _output.WriteLine($"Created {roleName} user: {username}");

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
