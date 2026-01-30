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
public class CountriesControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public CountriesControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All Countries Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "1")]
    public async Task GetAllCountries_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedCountries();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CountryDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} countries");
        foreach (var country in result)
        {
            Console.WriteLine($"  - {country.EnglishName} ({country.Code}): Active={country.IsActive}");
        }
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetAllCountries_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedCountries();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetAllCountries_WithSalesRole_ReturnsOk()
    {
        // Arrange
        await SeedCountries();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetAllCountries_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Countries", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetAllCountries_WithCustomerRole_ReturnsOk()
    {
        // Arrange
        await SeedCountries();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get Active Countries Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "1")]
    public async Task GetActiveCountries_WithAdminRole_ReturnsOnlyActive()
    {
        // Arrange
        await SeedCountries();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CountryDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.All(result, country => Assert.True(country.IsActive));

        Console.WriteLine($"Retrieved {result.Count()} active countries");
        foreach (var country in result)
        {
            Console.WriteLine($"  - {country.EnglishName} ({country.Code})");
        }
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetActiveCountries_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedCountries();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetActiveCountries_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Countries/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Country By Id Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "2")]
    public async Task GetCountryById_ValidId_ReturnsOk()
    {
        // Arrange
        var countryId = await SeedCountries();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Countries/{countryId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CountryDto>();
        Assert.NotNull(result);
        Assert.Equal(countryId, result.Id);
        Assert.NotEmpty(result.Code);
        Assert.NotEmpty(result.EnglishName);

        Console.WriteLine($"Retrieved country: {result.EnglishName}");
        Console.WriteLine($"  Code: {result.Code}");
        Console.WriteLine($"  TLD: {result.Tld}");
        Console.WriteLine($"  Local Name: {result.LocalName}");
        Console.WriteLine($"  Active: {result.IsActive}");
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetCountryById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetCountryById_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Countries/1", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetCountryById_WithCustomerRole_ReturnsOk()
    {
        // Arrange
        var countryId = await SeedCountries();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/Countries/{countryId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get Country By Code Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "2")]
    public async Task GetCountryByCode_ValidCode_ReturnsOk()
    {
        // Arrange
        await SeedCountries();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries/code/US", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CountryDto>();
        Assert.NotNull(result);
        Assert.Equal("US", result.Code);
        Assert.Equal("United States", result.EnglishName);

        Console.WriteLine($"Retrieved country by code: {result.EnglishName}");
        Console.WriteLine($"  Code: {result.Code}");
        Console.WriteLine($"  TLD: {result.Tld}");
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetCountryByCode_InvalidCode_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries/code/XX", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task GetCountryByCode_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/Countries/code/US", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Create Country Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "3")]
    public async Task CreateCountry_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCountryDto
        {
            Code = "CA",
            Tld = ".ca",
            EnglishName = "Canada",
            LocalName = "Canada",
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Countries", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CountryDto>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.Code, result.Code);
        Assert.Equal(createDto.Tld, result.Tld);
        Assert.Equal(createDto.EnglishName, result.EnglishName);
        Assert.Equal(createDto.LocalName, result.LocalName);
        Assert.Equal(createDto.IsActive, result.IsActive);
        Assert.True(result.CreatedAt > DateTime.MinValue);
        Assert.True(result.UpdatedAt > DateTime.MinValue);
        Assert.Equal(result.CreatedAt, result.UpdatedAt, TimeSpan.FromSeconds(1));

        Console.WriteLine($"Created country with ID: {result.Id}");
        Console.WriteLine($"  Code: {result.Code}");
        Console.WriteLine($"  English Name: {result.EnglishName}");
        Console.WriteLine($"  TLD: {result.Tld}");
        Console.WriteLine($"  CreatedAt: {result.CreatedAt}");
        Console.WriteLine($"  UpdatedAt: {result.UpdatedAt}");
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task CreateCountry_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCountryDto
        {
            Code = "TE",
            Tld = ".te",
            EnglishName = "Test Country",
            LocalName = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Countries", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task CreateCountry_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCountryDto
        {
            Code = "TE",
            Tld = ".te",
            EnglishName = "Test Country",
            LocalName = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Countries", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task CreateCountry_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateCountryDto
        {
            Code = "TE",
            Tld = ".te",
            EnglishName = "Test Country",
            LocalName = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Countries", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Update Country Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "4")]
    public async Task UpdateCountry_ValidData_ReturnsOk()
    {
        // Arrange
        var countryId = await SeedCountries();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Get the original country to compare timestamps
        var getResponse = await _client.GetAsync($"/api/v1/Countries/{countryId}", TestContext.Current.CancellationToken);
        var originalCountry = await getResponse.Content.ReadFromJsonAsync<CountryDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(originalCountry);

        // Wait a bit to ensure UpdatedAt will be different
        await Task.Delay(100, TestContext.Current.CancellationToken);

        var updateDto = new UpdateCountryDto
        {
            Code = "US",
            Tld = ".us",
            EnglishName = "United States of America",
            LocalName = "United States",
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Countries/{countryId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CountryDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(countryId, result.Id);
        Assert.Equal(updateDto.Code, result.Code);
        Assert.Equal(updateDto.Tld, result.Tld);
        Assert.Equal(updateDto.EnglishName, result.EnglishName);
        Assert.Equal(updateDto.LocalName, result.LocalName);
        Assert.Equal(updateDto.IsActive, result.IsActive);
        Assert.Equal(originalCountry.CreatedAt, result.CreatedAt); // CreatedAt should not change
        Assert.True(result.UpdatedAt > originalCountry.UpdatedAt); // UpdatedAt should be newer

        Console.WriteLine($"Updated country ID {result.Id}:");
        Console.WriteLine($"  New English Name: {result.EnglishName}");
        Console.WriteLine($"  New TLD: {result.Tld}");
        Console.WriteLine($"  CreatedAt: {result.CreatedAt} (unchanged)");
        Console.WriteLine($"  UpdatedAt: {result.UpdatedAt} (was: {originalCountry.UpdatedAt})");
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task UpdateCountry_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateCountryDto
        {
            Code = "XX",
            Tld = ".xx",
            EnglishName = "Updated Country",
            LocalName = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/Countries/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task UpdateCountry_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var countryId = await SeedCountries();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateCountryDto
        {
            Code = "US",
            Tld = ".us",
            EnglishName = "Updated Country",
            LocalName = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/Countries/{countryId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task UpdateCountry_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var updateDto = new UpdateCountryDto
        {
            Code = "US",
            Tld = ".us",
            EnglishName = "Updated Country",
            LocalName = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/Countries/1", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Delete Country Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "5")]
    public async Task DeleteCountry_ValidId_ReturnsNoContent()
    {
        // Arrange
        var countryId = await SeedCountries();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Countries/{countryId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Successfully deleted country ID: {countryId}");

        // Verify it's actually deleted
        var getResponse = await _client.GetAsync($"/api/v1/Countries/{countryId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task DeleteCountry_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/Countries/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task DeleteCountry_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var countryId = await SeedCountries();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/Countries/{countryId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Countries")]
    public async Task DeleteCountry_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/v1/Countries/1", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "6")]
    public async Task FullCRUDFlow_CreateReadUpdateDelete_Success()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        Console.WriteLine("=== Starting Full CRUD Flow for Countries ===");

        // Create
        Console.WriteLine("\n1. CREATE:");
        var createDto = new CreateCountryDto
        {
            Code = "AU",
            Tld = ".au",
            EnglishName = "Australia",
            LocalName = "Australia",
            IsActive = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/Countries", createDto, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<CountryDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(created);
        Console.WriteLine($"   Created ID: {created.Id}, Name: {created.EnglishName}");

        // Read
        Console.WriteLine("\n2. READ:");
        var readResponse = await _client.GetAsync($"/api/v1/Countries/{created.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);

        var read = await readResponse.Content.ReadFromJsonAsync<CountryDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(read);
        Assert.Equal(created.Id, read.Id);
        Assert.Equal(createDto.Code, read.Code);
        Assert.Equal(createDto.EnglishName, read.EnglishName);
        Console.WriteLine($"   Retrieved: {read.EnglishName} ({read.Code})");

        // Update
        Console.WriteLine("\n3. UPDATE:");
        var updateDto = new UpdateCountryDto
        {
            Code = "AU",
            Tld = ".au",
            EnglishName = "Commonwealth of Australia",
            LocalName = "Australia",
            IsActive = true
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/Countries/{created.Id}", updateDto, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<CountryDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(updated);
        Assert.Equal(updateDto.EnglishName, updated.EnglishName);
        Console.WriteLine($"   Updated to: {updated.EnglishName}");

        // Delete
        Console.WriteLine("\n4. DELETE:");
        var deleteResponse = await _client.DeleteAsync($"/api/v1/Countries/{created.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Console.WriteLine($"   Deleted ID: {created.Id}");

        // Verify deletion
        Console.WriteLine("\n5. VERIFY DELETION:");
        var verifyResponse = await _client.GetAsync($"/api/v1/Countries/{created.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, verifyResponse.StatusCode);
        Console.WriteLine("   Confirmed: Resource no longer exists");

        Console.WriteLine("\n=== Full CRUD Flow Completed Successfully ===");
    }

    [Fact]
    [Trait("Category", "Countries")]
    [Trait("Priority", "7")]
    public async Task ActiveFilter_OnlyReturnsActiveCountries()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create active country
        var activeDto = new CreateCountryDto
        {
            Code = "NZ",
            Tld = ".nz",
            EnglishName = "New Zealand",
            LocalName = "New Zealand",
            IsActive = true
        };
        await _client.PostAsJsonAsync("/api/v1/Countries", activeDto, TestContext.Current.CancellationToken);

        // Create inactive country
        var inactiveDto = new CreateCountryDto
        {
            Code = "XX",
            Tld = ".xx",
            EnglishName = "Inactive Country",
            LocalName = "Inactive",
            IsActive = false
        };
        await _client.PostAsJsonAsync("/api/v1/Countries", inactiveDto, TestContext.Current.CancellationToken);

        // Act
        var response = await _client.GetAsync("/api/v1/Countries/active", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CountryDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.All(result, country => Assert.True(country.IsActive));
        Assert.DoesNotContain(result, country => country.Code == "XX");

        Console.WriteLine($"Active filter returned {result.Count()} active countries");
        Console.WriteLine("Verified that inactive countries are excluded");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Seeds test countries in the database
    /// Returns the ID of the first seeded country
    /// </summary>
    private async Task<int> SeedCountries()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clear existing countries to avoid UNIQUE constraint violations
        var existingCountries = await context.Countries.ToListAsync();
        if (existingCountries.Any())
        {
            context.Countries.RemoveRange(existingCountries);
            await context.SaveChangesAsync();
        }

        var now = DateTime.UtcNow;
        var countries = new[]
        {
            new Country
            {
                Code = "US",
                Tld = ".us",
                EnglishName = "United States",
                LocalName = "United States",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Country
            {
                Code = "GB",
                Tld = ".uk",
                EnglishName = "United Kingdom",
                LocalName = "United Kingdom",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Country
            {
                Code = "FR",
                Tld = ".fr",
                EnglishName = "France",
                LocalName = "France",
                IsActive = false,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Countries.AddRangeAsync(countries);
        await context.SaveChangesAsync();

        return countries[0].Id;
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

