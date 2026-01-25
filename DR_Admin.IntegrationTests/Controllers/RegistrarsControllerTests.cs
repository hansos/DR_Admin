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
public class RegistrarsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly TestWebApplicationFactory _factory;

    public RegistrarsControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    [Trait("Category", "Registrars")]
    public async Task GetTldsByRegistrar_ValidId_ReturnsOk()
    {
        // Arrange
        var registrarId = await SeedRegistrar();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Registrars/{registrarId}/tlds");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _output.WriteLine($"Retrieved TLDs for registrar {registrarId}");
    }

    [Fact]
    [Trait("Category", "Registrars")]
    public async Task GetTldsByRegistrar_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Registrars/1/tlds");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<int> SeedRegistrar()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var registrar = new Registrar
        {
            Name = "Test Registrar",
            Code = "TEST",
            IsActive = true,
            Website = "https://test.example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Registrars.AddAsync(registrar);
        await context.SaveChangesAsync();

        return registrar.Id;
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
            Address = "123 Test St",
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
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return result!.AccessToken;
    }
}
