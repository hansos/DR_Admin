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
public class DnsRecordsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public DnsRecordsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Get All DNS Records Tests

    [Fact]
    [Trait("Category", "DnsRecords")]
    [Trait("Priority", "1")]
    public async Task GetAllDnsRecords_WithAdminRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsRecordDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Console.WriteLine($"Retrieved {result.Count()} DNS records");
        foreach (var record in result)
        {
            Console.WriteLine($"  - {record.Name} ({record.Type}): {record.Value}");
        }
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetAllDnsRecords_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedDnsRecords();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetAllDnsRecords_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        await SeedDnsRecords();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetAllDnsRecords_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get DNS Record By Id Tests

    [Fact]
    [Trait("Category", "DnsRecords")]
    [Trait("Priority", "2")]
    public async Task GetDnsRecordById_ValidId_ReturnsOk()
    {
        // Arrange
        var recordId = await SeedDnsRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(recordId, result.Id);

        Console.WriteLine($"Retrieved DNS record: {result.Name} ({result.Type})");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetDnsRecordById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetDnsRecordById_WithCustomerRole_ReturnsOk()
    {
        // Arrange
        var recordId = await SeedDnsRecords();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get DNS Records By Domain Id Tests

    [Fact]
    [Trait("Category", "DnsRecords")]
    [Trait("Priority", "3")]
    public async Task GetDnsRecordsByDomainId_ValidDomainId_ReturnsOk()
    {
        // Arrange
        var domainId = await SeedDomainWithRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsRecords/domain/{domainId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsRecordDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, record => Assert.Equal(domainId, record.DomainId));

        Console.WriteLine($"Retrieved {result.Count()} DNS records for domain {domainId}");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetDnsRecordsByDomainId_EmptyDomain_ReturnsEmptyList()
    {
        // Arrange
        var domainId = await SeedDomainWithoutRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsRecords/domain/{domainId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsRecordDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetDnsRecordsByDomainId_WithCustomerRole_ReturnsOk()
    {
        // Arrange
        var domainId = await SeedDomainWithRecords();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/DnsRecords/domain/{domainId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get DNS Records By Type Tests

    [Fact]
    [Trait("Category", "DnsRecords")]
    [Trait("Priority", "4")]
    public async Task GetDnsRecordsByType_ARecords_ReturnsOnlyARecords()
    {
        // Arrange
        await SeedMultipleRecordTypes();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords/type/A", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<DnsRecordDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, record => Assert.Equal("A", record.Type));

        Console.WriteLine($"Retrieved {result.Count()} A records");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetDnsRecordsByType_WithSupportRole_ReturnsOk()
    {
        // Arrange
        await SeedMultipleRecordTypes();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords/type/CNAME", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task GetDnsRecordsByType_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/DnsRecords/type/A", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Create DNS Record Tests

    [Fact]
    [Trait("Category", "DnsRecords")]
    [Trait("Priority", "5")]
    public async Task CreateDnsRecord_ValidData_ReturnsCreated()
    {
        // Arrange
        var (domainId, recordTypeId) = await SeedDomainAndRecordType();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "www",
            Value = "192.0.2.100",
            TTL = 3600
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsRecords", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.DomainId, result.DomainId);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Value, result.Value);

        Console.WriteLine($"Created DNS record with ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task CreateDnsRecord_WithMxRecord_IncludesPriority()
    {
        // Arrange
        var (domainId, mxRecordTypeId) = await SeedDomainAndMxRecordType();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = mxRecordTypeId,
            Name = "@",
            Value = "mail.example.com",
            TTL = 3600,
            Priority = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsRecords", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(10, result.Priority);

        Console.WriteLine($"Created MX record with priority {result.Priority}");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task CreateDnsRecord_WithCustomerRole_ReturnsCreated()
    {
        // Arrange
        var (domainId, recordTypeId) = await SeedDomainAndRecordType();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.50",
            TTL = 3600
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsRecords", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task CreateDnsRecord_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDnsRecordDto
        {
            // Missing required fields
            DomainId = 0,
            DnsRecordTypeId = 0,
            Name = "",
            Value = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/DnsRecords", createDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Update DNS Record Tests

    [Fact]
    [Trait("Category", "DnsRecords")]
    [Trait("Priority", "6")]
    public async Task UpdateDnsRecord_ValidData_ReturnsOk()
    {
        // Arrange
        var (recordId, domainId, recordTypeId) = await SeedEditableRecord();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "updated",
            Value = "192.0.2.200",
            TTL = 7200
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsRecords/{recordId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DnsRecordDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(recordId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Value, result.Value);
        Assert.Equal(updateDto.TTL, result.TTL);

        Console.WriteLine($"Updated DNS record ID: {result.Id}");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task UpdateDnsRecord_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var (_, domainId, recordTypeId) = await SeedEditableRecord();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.1",
            TTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/DnsRecords/99999", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task UpdateDnsRecord_NonEditableRecord_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var (recordId, domainId, recordTypeId) = await SeedNonEditableRecord();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.1",
            TTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsRecords/{recordId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task UpdateDnsRecord_NonEditableRecord_WithAdminRole_ReturnsOk()
    {
        // Arrange
        var (recordId, domainId, recordTypeId) = await SeedNonEditableRecord();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "admin-updated",
            Value = "192.0.2.250",
            TTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsRecords/{recordId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Console.WriteLine("Admin successfully updated non-editable record");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task UpdateDnsRecord_WithCustomerRole_EditableRecord_ReturnsOk()
    {
        // Arrange
        var (recordId, domainId, recordTypeId) = await SeedEditableRecord();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateDnsRecordDto
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "customer-updated",
            Value = "192.0.2.150",
            TTL = 3600
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/DnsRecords/{recordId}", updateDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Delete DNS Record Tests

    [Fact]
    [Trait("Category", "DnsRecords")]
    [Trait("Priority", "7")]
    public async Task DeleteDnsRecord_ValidId_ReturnsNoContent()
    {
        // Arrange
        var recordId = await SeedDnsRecords();
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Console.WriteLine($"Deleted DNS record ID: {recordId}");
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task DeleteDnsRecord_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/DnsRecords/99999", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task DeleteDnsRecord_WithSupportRole_ReturnsNoContent()
    {
        // Arrange
        var recordId = await SeedDnsRecords();
        var token = await GetSupportTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "DnsRecords")]
    public async Task DeleteDnsRecord_WithCustomerRole_ReturnsForbidden()
    {
        // Arrange
        var recordId = await SeedDnsRecords();
        var token = await GetCustomerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/DnsRecords/{recordId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<int> SeedDnsRecords()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean up existing test data
        var existingRecords = await context.DnsRecords.ToListAsync();
        context.DnsRecords.RemoveRange(existingRecords);
        await context.SaveChangesAsync();

        var (domainId, recordTypeId) = await SeedDomainAndRecordType();

        var record = new DnsRecord
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "@",
            Value = "192.0.2.1",
            TTL = 3600,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsRecords.Add(record);
        await context.SaveChangesAsync();

        return record.Id;
    }

    private async Task<int> SeedDomainWithRecords()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (domainId, recordTypeId) = await SeedDomainAndRecordType();

        var records = new List<DnsRecord>
        {
            new DnsRecord
            {
                DomainId = domainId,
                DnsRecordTypeId = recordTypeId,
                Name = "@",
                Value = "192.0.2.1",
                TTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new DnsRecord
            {
                DomainId = domainId,
                DnsRecordTypeId = recordTypeId,
                Name = "www",
                Value = "192.0.2.2",
                TTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.DnsRecords.AddRange(records);
        await context.SaveChangesAsync();

        return domainId;
    }

    private async Task<int> SeedDomainWithoutRecords()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (domainId, _) = await SeedDomainAndRecordType();
        return domainId;
    }

    private async Task SeedMultipleRecordTypes()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean up
        var existingRecords = await context.DnsRecords.ToListAsync();
        context.DnsRecords.RemoveRange(existingRecords);
        await context.SaveChangesAsync();

        var (domainId, aRecordTypeId) = await SeedDomainAndRecordType();

        // Ensure CNAME record type exists
        var cnameRecordType = await context.DnsRecordTypes.FirstOrDefaultAsync(t => t.Type == "CNAME");
        if (cnameRecordType == null)
        {
            cnameRecordType = new DnsRecordType
            {
                Type = "CNAME",
                Description = "Canonical name record",
                HasPriority = false,
                HasWeight = false,
                HasPort = false,
                IsEditableByUser = true,
                IsActive = true,
                DefaultTTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.DnsRecordTypes.Add(cnameRecordType);
            await context.SaveChangesAsync();
        }

        var records = new List<DnsRecord>
        {
            new DnsRecord
            {
                DomainId = domainId,
                DnsRecordTypeId = aRecordTypeId,
                Name = "@",
                Value = "192.0.2.1",
                TTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new DnsRecord
            {
                DomainId = domainId,
                DnsRecordTypeId = cnameRecordType.Id,
                Name = "www",
                Value = "example.com",
                TTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.DnsRecords.AddRange(records);
        await context.SaveChangesAsync();
    }

    private async Task<(int domainId, int recordTypeId)> SeedDomainAndRecordType()
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

        // Create test domain
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

        var timestamp = DateTime.UtcNow.Ticks;
        var domain = new Domain
        {
            CustomerId = customer.Id,
            RegistrarId = registrar.Id,
            Name = $"test-{timestamp}.com",
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

        return (domain.Id, aRecordType.Id);
    }

    private async Task<(int domainId, int mxRecordTypeId)> SeedDomainAndMxRecordType()
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

        var (domainId, _) = await SeedDomainAndRecordType();
        return (domainId, mxRecordType.Id);
    }

    private async Task<(int recordId, int domainId, int recordTypeId)> SeedEditableRecord()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (domainId, recordTypeId) = await SeedDomainAndRecordType();

        var record = new DnsRecord
        {
            DomainId = domainId,
            DnsRecordTypeId = recordTypeId,
            Name = "test",
            Value = "192.0.2.100",
            TTL = 3600,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsRecords.Add(record);
        await context.SaveChangesAsync();

        return (record.Id, domainId, recordTypeId);
    }

    private async Task<(int recordId, int domainId, int recordTypeId)> SeedNonEditableRecord()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create non-editable record type
        var nsRecordType = await context.DnsRecordTypes.FirstOrDefaultAsync(t => t.Type == "NS");
        if (nsRecordType == null)
        {
            nsRecordType = new DnsRecordType
            {
                Type = "NS",
                Description = "Name server record",
                HasPriority = false,
                HasWeight = false,
                HasPort = false,
                IsEditableByUser = false, // Not editable by users
                IsActive = true,
                DefaultTTL = 3600,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.DnsRecordTypes.Add(nsRecordType);
            await context.SaveChangesAsync();
        }

        var (domainId, _) = await SeedDomainAndRecordType();

        var record = new DnsRecord
        {
            DomainId = domainId,
            DnsRecordTypeId = nsRecordType.Id,
            Name = "@",
            Value = "ns1.example.com",
            TTL = 3600,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.DnsRecords.Add(record);
        await context.SaveChangesAsync();

        return (record.Id, domainId, nsRecordType.Id);
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

