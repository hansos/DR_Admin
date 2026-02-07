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
public class RegistrarTldsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public RegistrarTldsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    [Trait("Category", "RegistrarTlds")]
    public async Task GetAllRegistrarTlds_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedRegistrarTlds();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/RegistrarTlds", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Console.WriteLine("Retrieved registrar TLDs");
    }

    [Fact]
    [Trait("Category", "RegistrarTlds")]
    public async Task GetAllRegistrarTlds_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/RegistrarTlds", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<int> SeedRegistrarTlds()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var registrar = new Registrar
        {
            Name = "Test Registrar",
            Code = "TEST",
            IsActive = true,
            Website = "https://test.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Registrars.AddAsync(registrar);

        var tld = new Tld
        {
            Extension = ".test",
            Description = "Test TLD",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Tlds.AddAsync(tld);
        await context.SaveChangesAsync();

        var registrarTld = new RegistrarTld
        {
            RegistrarId = registrar.Id,
            TldId = tld.Id,
            IsActive = true,
            AutoRenew = false,
            MinRegistrationYears = 1,
            MaxRegistrationYears = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.RegistrarTlds.AddAsync(registrarTld);
        await context.SaveChangesAsync();

        return registrarTld.Id;
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var (username, _) = await CreateUserWithRole("Admin");
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
            Name = $"Test Customer {timestamp}",
            Email = $"test{timestamp}@example.com",
            Phone = "555-0100",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        var username = $"{roleName.ToLower()}user{timestamp}";
        var user = new User
        {
            CustomerId = customer.Id,
            Username = username,
            Email = customer.Email,
            PasswordHash = "Test@1234",
            EmailConfirmed = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var userRole = new UserRole { UserId = user.Id, RoleId = role.Id };
        await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        return (username, customer.Email);
    }

    private async Task<string> LoginAndGetTokenAsync(string username)
    {
        var loginRequest = new LoginRequestDto { Username = username, Password = "Test@1234" };
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(TestContext.Current.CancellationToken);
        return result!.AccessToken;
    }
}

