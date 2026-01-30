using System.Net;
using System.Net.Http.Json;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace DR_Admin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class InitializationControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public InitializationControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Initialize System Tests

    [Fact]
    [Trait("Category", "Initialization")]
    [Trait("Priority", "1")]
    public async Task Initialize_WithValidData_ReturnsOk()
    {
        // Arrange
        await CleanupAllUsers();

        var request = new InitializationRequestDto
        {
            Username = $"firstadmin{DateTime.UtcNow.Ticks}",
            Password = "Admin@123456",
            Email = $"admin{DateTime.UtcNow.Ticks}@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Initialization/initialize", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<InitializationResponseDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(request.Username, result.Username);

        Console.WriteLine($"System initialized with user: {result.Username}");
    }

    [Fact]
    [Trait("Category", "Initialization")]
    public async Task Initialize_WhenUsersAlreadyExist_ReturnsBadRequest()
    {
        // Arrange
        await EnsureUserExists();

        var request = new InitializationRequestDto
        {
            Username = "anotherAdmin",
            Password = "Admin@123456",
            Email = "another@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Initialization/initialize", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Console.WriteLine("Initialization correctly rejected when users already exist");
    }

    [Fact]
    [Trait("Category", "Initialization")]
    public async Task Initialize_WithMissingUsername_ReturnsBadRequest()
    {
        // Arrange
        await CleanupAllUsers();

        var request = new InitializationRequestDto
        {
            Username = "",
            Password = "Admin@123456",
            Email = "admin@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Initialization/initialize", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Username", content, StringComparison.OrdinalIgnoreCase);

        Console.WriteLine("Initialization correctly rejected when username is missing");
    }

    [Fact]
    [Trait("Category", "Initialization")]
    public async Task Initialize_WithMissingPassword_ReturnsBadRequest()
    {
        // Arrange
        await CleanupAllUsers();

        var request = new InitializationRequestDto
        {
            Username = "admin",
            Password = "",
            Email = "admin@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Initialization/initialize", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("password", content, StringComparison.OrdinalIgnoreCase);

        Console.WriteLine("Initialization correctly rejected when password is missing");
    }

    [Fact]
    [Trait("Category", "Initialization")]
    public async Task Initialize_WithMissingEmail_ReturnsBadRequest()
    {
        // Arrange
        await CleanupAllUsers();

        var request = new InitializationRequestDto
        {
            Username = "admin",
            Password = "Admin@123456",
            Email = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Initialization/initialize", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("email", content, StringComparison.OrdinalIgnoreCase);

        Console.WriteLine("Initialization correctly rejected when email is missing");
    }

    [Fact]
    [Trait("Category", "Initialization")]
    public async Task Initialize_WithAllRequiredFields_CreatesAdminRole()
    {
        // Arrange
        await CleanupAllUsers();

        var request = new InitializationRequestDto
        {
            Username = $"admin{DateTime.UtcNow.Ticks}",
            Password = "Admin@123456",
            Email = $"admin{DateTime.UtcNow.Ticks}@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Initialization/initialize", request, TestContext.Current.CancellationToken    );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify Admin role was created
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin", TestContext.Current.CancellationToken);
        Assert.NotNull(adminRole);

        Console.WriteLine("Admin role successfully created during initialization");
    }

    [Fact]
    [Trait("Category", "Initialization")]
    public async Task Initialize_NoAuthenticationRequired_AllowsAnonymous()
    {
        // Arrange
        await CleanupAllUsers();

        var request = new InitializationRequestDto
        {
            Username = $"admin{DateTime.UtcNow.Ticks}",
            Password = "Admin@123456",
            Email = $"admin{DateTime.UtcNow.Ticks}@example.com"
        };

        // Act - No authorization header set
        var response = await _client.PostAsJsonAsync("/api/v1/Initialization/initialize", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Console.WriteLine("Initialization endpoint correctly allows anonymous access");
    }

    #endregion

    #region Helper Methods

    private async Task CleanupAllUsers()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Remove all user roles
        var userRoles = await context.UserRoles.ToListAsync();
        context.UserRoles.RemoveRange(userRoles);

        // Remove all users
        var users = await context.Users.ToListAsync();
        context.Users.RemoveRange(users);

        // Remove all customers
        var customers = await context.Customers.ToListAsync();
        context.Customers.RemoveRange(customers);

        await context.SaveChangesAsync();

        Console.WriteLine("Cleaned up all users for initialization test");
    }

    private async Task EnsureUserExists()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Check if any user exists
        var userExists = await context.Users.AnyAsync();
        if (userExists)
        {
            return;
        }

        // Create a test user
        var customer = new Customer
        {
            Name = "Test Customer",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var user = new User
        {
            CustomerId = customer.Id,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "Test@1234",
            EmailConfirmed = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        Console.WriteLine("Created test user to prevent initialization");
    }

    #endregion
}

