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
public class ContactPersonsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public ContactPersonsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All ContactPersons Tests

    [Fact]
    [Trait("Category", "ContactPersons")]
    [Trait("Priority", "1")]
    public async Task GetAllContactPersons_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedContactPersons();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ContactPersons");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ContactPersonDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} contact persons");
        foreach (var contactPerson in result.Take(5))
        {
            Console.WriteLine($"  - {contactPerson.FirstName} {contactPerson.LastName} ({contactPerson.Email})");
        }
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task GetAllContactPersons_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedContactPersons();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ContactPersons");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task GetAllContactPersons_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/ContactPersons");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get ContactPersons By Customer Id Tests

    [Fact]
    [Trait("Category", "ContactPersons")]
    [Trait("Priority", "2")]
    public async Task GetContactPersonsByCustomerId_ValidId_ReturnsOk()
    {
        // Arrange
        var (customerId, _) = await SeedContactPersons();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/ContactPersons/customer/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ContactPersonDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, cp => Assert.Equal(customerId, cp.CustomerId));

        Console.WriteLine($"Retrieved {result.Count()} contact persons for customer {customerId}");
    }

    #endregion

    #region Get ContactPerson By Id Tests

    [Fact]
    [Trait("Category", "ContactPersons")]
    [Trait("Priority", "2")]
    public async Task GetContactPersonById_ValidId_ReturnsOk()
    {
        // Arrange
        var (customerId, contactPersonId) = await SeedContactPersons();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/ContactPersons/{contactPersonId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ContactPersonDto>();
        Assert.NotNull(result);
        Assert.Equal(contactPersonId, result.Id);
        Assert.NotEmpty(result.FirstName);
        Assert.NotEmpty(result.LastName);

        Console.WriteLine($"Retrieved contact person: {result.FirstName} {result.LastName} ({result.Email})");
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task GetContactPersonById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/ContactPersons/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task GetContactPersonById_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/ContactPersons/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Create ContactPerson Tests

    [Fact]
    [Trait("Category", "ContactPersons")]
    [Trait("Priority", "3")]
    public async Task CreateContactPerson_ValidData_ReturnsCreated()
    {
        // Arrange
        var customerId = await SeedCustomer();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateContactPersonDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "555-1234",
            Position = "Manager",
            Department = "Sales",
            IsPrimary = true,
            IsActive = true,
            Notes = "Primary contact",
            CustomerId = customerId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ContactPersons", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ContactPersonDto>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.FirstName, result.FirstName);
        Assert.Equal(createDto.LastName, result.LastName);
        Assert.Equal(createDto.Email, result.Email);
        Assert.Equal(createDto.CustomerId, result.CustomerId);
        Assert.True(result.CreatedAt > DateTime.MinValue);

        Console.WriteLine($"Created contact person with ID: {result.Id}");
        Console.WriteLine($"  Name: {result.FirstName} {result.LastName}");
        Console.WriteLine($"  Email: {result.Email}");
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task CreateContactPerson_WithSalesRole_ReturnsCreated()
    {
        // Arrange
        var customerId = await SeedCustomer();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateContactPersonDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "555-5678",
            CustomerId = customerId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ContactPersons", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task CreateContactPerson_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var customerId = await SeedCustomer();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateContactPersonDto
        {
            FirstName = "Support",
            LastName = "User",
            Email = "support@example.com",
            Phone = "555-0000",
            CustomerId = customerId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ContactPersons", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task CreateContactPerson_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateContactPersonDto
        {
            FirstName = "Test",
            LastName = "Person",
            Email = "test@example.com",
            Phone = "555-0000",
            CustomerId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ContactPersons", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Update ContactPerson Tests

    [Fact]
    [Trait("Category", "ContactPersons")]
    [Trait("Priority", "4")]
    public async Task UpdateContactPerson_ValidData_ReturnsOk()
    {
        // Arrange
        var (customerId, contactPersonId) = await SeedContactPersons();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateContactPersonDto
        {
            FirstName = "Updated",
            LastName = "Person",
            Email = "updated@example.com",
            Phone = "555-9999",
            Position = "Director",
            Department = "Marketing",
            IsPrimary = false,
            IsActive = true,
            Notes = "Updated contact",
            CustomerId = customerId
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/ContactPersons/{contactPersonId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ContactPersonDto>();
        Assert.NotNull(result);
        Assert.Equal(contactPersonId, result.Id);
        Assert.Equal(updateDto.FirstName, result.FirstName);
        Assert.Equal(updateDto.LastName, result.LastName);
        Assert.Equal(updateDto.Email, result.Email);

        Console.WriteLine($"Updated contact person ID {result.Id}:");
        Console.WriteLine($"  New Name: {result.FirstName} {result.LastName}");
        Console.WriteLine($"  New Email: {result.Email}");
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task UpdateContactPerson_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var customerId = await SeedCustomer();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateContactPersonDto
        {
            FirstName = "Updated",
            LastName = "Person",
            Email = "updated@example.com",
            Phone = "555-0000",
            CustomerId = customerId
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/ContactPersons/99999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task UpdateContactPerson_WithSupportRole_ReturnsForbidden()
    {
        // Arrange
        var (customerId, contactPersonId) = await SeedContactPersons();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateContactPersonDto
        {
            FirstName = "Updated",
            LastName = "Person",
            Email = "updated@example.com",
            Phone = "555-0000",
            CustomerId = customerId
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/ContactPersons/{contactPersonId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Delete ContactPerson Tests

    [Fact]
    [Trait("Category", "ContactPersons")]
    [Trait("Priority", "5")]
    public async Task DeleteContactPerson_ValidId_ReturnsNoContent()
    {
        // Arrange
        var contactPersonId = await SeedContactPersonForDelete();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/ContactPersons/{contactPersonId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted contact person with ID: {contactPersonId}");
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task DeleteContactPerson_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/ContactPersons/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "ContactPersons")]
    public async Task DeleteContactPerson_WithSalesRole_ReturnsForbidden()
    {
        // Arrange
        var contactPersonId = await SeedContactPersonForDelete();
        var token = await GetSalesTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/ContactPersons/{contactPersonId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("Category", "ContactPersons")]
    [Trait("Priority", "6")]
    public async Task FullCRUDFlow_CreateReadUpdateDelete_Success()
    {
        // Arrange
        var customerId = await SeedCustomer();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        Console.WriteLine("=== Starting Full CRUD Flow for ContactPersons ===");

        // Create
        Console.WriteLine("\n1. CREATE:");
        var createDto = new CreateContactPersonDto
        {
            FirstName = "CRUD",
            LastName = "Test",
            Email = "crudtest@example.com",
            Phone = "555-CRUD",
            Position = "Tester",
            Department = "QA",
            IsPrimary = true,
            IsActive = true,
            Notes = "CRUD test contact",
            CustomerId = customerId
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/ContactPersons", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ContactPersonDto>();
        Assert.NotNull(created);
        Console.WriteLine($"   Created ID: {created.Id}, Name: {created.FirstName} {created.LastName}");

        // Read
        Console.WriteLine("\n2. READ:");
        var readResponse = await _client.GetAsync($"/api/v1/ContactPersons/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);

        var read = await readResponse.Content.ReadFromJsonAsync<ContactPersonDto>();
        Assert.NotNull(read);
        Assert.Equal(created.Id, read.Id);
        Assert.Equal(createDto.FirstName, read.FirstName);
        Assert.Equal(createDto.LastName, read.LastName);
        Console.WriteLine($"   Retrieved: {read.FirstName} {read.LastName} ({read.Email})");

        // Update
        Console.WriteLine("\n3. UPDATE:");
        var updateDto = new UpdateContactPersonDto
        {
            FirstName = "Updated",
            LastName = "CRUD",
            Email = "crudupdated@example.com",
            Phone = "555-UP",
            Position = "Senior Tester",
            Department = "Quality Assurance",
            IsPrimary = false,
            IsActive = true,
            Notes = "Updated CRUD test",
            CustomerId = customerId
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/ContactPersons/{created.Id}", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<ContactPersonDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateDto.FirstName, updated.FirstName);
        Assert.Equal(updateDto.LastName, updated.LastName);
        Assert.Equal(updateDto.Email, updated.Email);
        Console.WriteLine($"   Updated to: {updated.FirstName} {updated.LastName} ({updated.Email})");

        // Delete
        Console.WriteLine("\n4. DELETE:");
        var deleteResponse = await _client.DeleteAsync($"/api/v1/ContactPersons/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Console.WriteLine($"   Deleted ID: {created.Id}");

        // Verify deletion
        Console.WriteLine("\n5. VERIFY DELETION:");
        var verifyResponse = await _client.GetAsync($"/api/v1/ContactPersons/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, verifyResponse.StatusCode);
        Console.WriteLine("   Confirmed: Resource no longer exists");

        Console.WriteLine("\n=== Full CRUD Flow Completed Successfully ===");
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedCustomer()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var customer = new Customer
        {
            Name = "Test Customer",
            Email = "testcustomer@example.com",
            Phone = "555-0001",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            PostalCode = "12345",
            IsActive = true,
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        return customer.Id;
    }

    private async Task<(int customerId, int contactPersonId)> SeedContactPersons()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var customer = new Customer
        {
            Name = "Test Customer",
            Email = "testcustomer@example.com",
            Phone = "555-0001",
            IsActive = true,
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        var contactPersons = new[]
        {
            new ContactPerson
            {
                FirstName = "Test",
                LastName = "Contact1",
                Email = "contact1@example.com",
                Phone = "555-0001",
                Position = "Manager",
                IsPrimary = true,
                IsActive = true,
                CustomerId = customer.Id,
                CreatedAt = now,
                UpdatedAt = now
            },
            new ContactPerson
            {
                FirstName = "Test",
                LastName = "Contact2",
                Email = "contact2@example.com",
                Phone = "555-0002",
                Position = "Assistant",
                IsPrimary = false,
                IsActive = true,
                CustomerId = customer.Id,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.ContactPersons.AddRangeAsync(contactPersons);
        await context.SaveChangesAsync();

        return (customer.Id, contactPersons[0].Id);
    }

    private async Task<int> SeedContactPersonForDelete()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;
        var customer = new Customer
        {
            Name = "Delete Test Customer",
            Email = "deletecustomer@example.com",
            Phone = "555-DEL",
            IsActive = true,
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        var contactPerson = new ContactPerson
        {
            FirstName = "Delete",
            LastName = "Test",
            Email = "delete@example.com",
            Phone = "555-DEL",
            IsActive = true,
            CustomerId = customer.Id,
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.ContactPersons.AddAsync(contactPerson);
        await context.SaveChangesAsync();

        return contactPerson.Id;
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

