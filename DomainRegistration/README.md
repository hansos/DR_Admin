# Domain Registration Library

A flexible library for domain registration, renewal, and DNS management across multiple registrars.

## Features

- **Domain Operations**
  - Check domain availability
  - Register new domains
  - Renew existing domains
  - Transfer domains from other registrars
  - Get domain information
  - Update domain settings (nameservers, privacy, auto-renew)

- **DNS Management**
  - Get DNS zone records
  - Add DNS records
  - Update DNS records
  - Delete DNS records
  - Bulk update DNS zones

## Supported Registrars

- **Namecheap** - Full implementation with API support
- **GoDaddy** - Full implementation with REST API
- **Cloudflare** - Full implementation with Cloudflare API
- **Generic** - Template for custom registrar implementations

## Usage

### Basic Setup

```csharp
using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Infrastructure.Settings;
using DomainRegistrationLib.Models;

// Configure settings
var settings = new RegistrarSettings
{
    Provider = "godaddy",
    GoDaddy = new GoDaddySettings
    {
        ApiKey = "your-api-key",
        ApiSecret = "your-api-secret",
        UseProduction = false // Use test environment
    }
};

// Create factory and registrar
var factory = new DomainRegistrarFactory(settings);
var registrar = factory.CreateRegistrar();
```

### Check Domain Availability

```csharp
var result = await registrar.CheckAvailabilityAsync("example.com");
if (result.Success && result.IsAvailable)
{
    Console.WriteLine($"{result.DomainName} is available!");
}
```

### Register a Domain

```csharp
var request = new DomainRegistrationRequest
{
    DomainName = "example.com",
    Years = 1,
    PrivacyProtection = true,
    AutoRenew = true,
    RegistrantContact = new ContactInformation
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Phone = "+1.1234567890",
        Address1 = "123 Main St",
        City = "Springfield",
        State = "IL",
        PostalCode = "62701",
        Country = "US"
    }
};

var result = await registrar.RegisterDomainAsync(request);
if (result.Success)
{
    Console.WriteLine($"Domain registered! Expires: {result.ExpirationDate}");
}
```

### Renew a Domain

```csharp
var request = new DomainRenewalRequest
{
    DomainName = "example.com",
    Years = 1
};

var result = await registrar.RenewDomainAsync(request);
if (result.Success)
{
    Console.WriteLine($"Domain renewed! New expiration: {result.NewExpirationDate}");
}
```

### Manage DNS Records

```csharp
// Get current DNS zone
var zone = await registrar.GetDnsZoneAsync("example.com");

// Add a new A record
var record = new DnsRecordModel
{
    Type = "A",
    Name = "www",
    Value = "192.0.2.1",
    TTL = 3600
};

var result = await registrar.AddDnsRecordAsync("example.com", record);
if (result.Success)
{
    Console.WriteLine("DNS record added successfully!");
}

// Add an MX record with priority
var mxRecord = new DnsRecordModel
{
    Type = "MX",
    Name = "@",
    Value = "mail.example.com",
    TTL = 3600,
    Priority = 10
};

await registrar.AddDnsRecordAsync("example.com", mxRecord);
```

### Update Domain Settings

```csharp
// Update nameservers
var nameservers = new List<string>
{
    "ns1.example.com",
    "ns2.example.com"
};
await registrar.UpdateNameserversAsync("example.com", nameservers);

// Enable privacy protection
await registrar.SetPrivacyProtectionAsync("example.com", true);

// Enable auto-renew
await registrar.SetAutoRenewAsync("example.com", true);
```

## Configuration Examples

### Namecheap

```csharp
var settings = new RegistrarSettings
{
    Provider = "namecheap",
    Namecheap = new NamecheapSettings
    {
        ApiUser = "your-api-user",
        ApiKey = "your-api-key",
        Username = "your-username",
        ClientIp = "your-whitelisted-ip",
        UseSandbox = true
    }
};
```

### GoDaddy

```csharp
var settings = new RegistrarSettings
{
    Provider = "godaddy",
    GoDaddy = new GoDaddySettings
    {
        ApiKey = "your-api-key",
        ApiSecret = "your-api-secret",
        UseProduction = false
    }
};
```

### Cloudflare

```csharp
var settings = new RegistrarSettings
{
    Provider = "cloudflare",
    Cloudflare = new CloudflareSettings
    {
        ApiToken = "your-api-token",
        AccountId = "your-account-id"
    }
};
```

## Integration with ISPAdmin

The library is designed to work seamlessly with the ISPAdmin database structure:

- **Registrar** entity stores registrar credentials and configuration
- **Domain** entity tracks registered domains
- **RegistrarTld** entity defines pricing and availability per TLD
- **DnsRecord** entity stores DNS records

The main application can use the factory to create registrars dynamically based on the database configuration:

```csharp
// Get registrar from database
var registrar = await _context.Registrars.FindAsync(registrarId);

// Create settings from database entity
var settings = new RegistrarSettings
{
    Provider = registrar.Code.ToLower(),
    // Map other settings based on registrar.Code
};

// Create and use registrar
var factory = new DomainRegistrarFactory(settings);
var domainRegistrar = factory.CreateRegistrar();

// Now use domainRegistrar for operations
var result = await domainRegistrar.RegisterDomainAsync(request);
```

## Creating Custom Registrar Implementations

To add support for a new registrar:

1. Create a settings class in `Infrastructure/Settings/`
2. Add the settings to `RegistrarSettings.cs`
3. Create an implementation class inheriting from `BaseRegistrar`
4. Implement all required methods
5. Add the registrar to `DomainRegistrarFactory.cs`

Example:

```csharp
public class MyRegistrarSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = "https://api.myregistrar.com";
}

public class MyRegistrar : BaseRegistrar
{
    private readonly string _apiKey;

    public MyRegistrar(string apiKey, string apiUrl)
        : base(apiUrl)
    {
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
    }

    public override async Task<DomainAvailabilityResult> CheckAvailabilityAsync(string domainName)
    {
        // Implementation here
    }

    // Implement other required methods...
}
```

## Error Handling

All methods return result objects with error information:

```csharp
var result = await registrar.RegisterDomainAsync(request);
if (!result.Success)
{
    Console.WriteLine($"Error: {result.Message}");
    Console.WriteLine($"Error Code: {result.ErrorCode}");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}
```

## License

Part of the ISPAdmin system for domain resellers.
