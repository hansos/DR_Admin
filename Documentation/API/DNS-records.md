# DNS Records API Documentation

## Overview

The DNS Records API provides comprehensive management of DNS records for different types including A, AAAA, CNAME, MX, TXT, NS, SRV, and other standard DNS record types.

## DNS Record Types Supported

### Common DNS Record Types

1. **A Record** - Maps a domain name to an IPv4 address
   - Example: `example.com -> 192.0.2.1`

2. **AAAA Record** - Maps a domain name to an IPv6 address
   - Example: `example.com -> 2001:0db8:85a3:0000:0000:8a2e:0370:7334`

3. **CNAME Record** - Creates an alias from one domain to another
   - Example: `www.example.com -> example.com`

4. **MX Record** - Specifies mail servers for the domain
   - Requires: Priority field
   - Example: `example.com -> mail.example.com` (Priority: 10)

5. **TXT Record** - Stores text information (often used for SPF, DKIM, domain verification)
   - Example: `example.com -> "v=spf1 include:_spf.google.com ~all"`

6. **NS Record** - Specifies authoritative name servers for the domain
   - Example: `example.com -> ns1.nameserver.com`

7. **SRV Record** - Specifies location of services
   - Requires: Priority, Weight, Port fields
   - Example: `_sip._tcp.example.com -> sipserver.example.com` (Priority: 10, Weight: 60, Port: 5060)

8. **CAA Record** - Specifies which Certificate Authorities can issue certificates
   - Example: `example.com -> "0 issue letsencrypt.org"`

9. **SOA Record** - Start of Authority record
10. **PTR Record** - Pointer record for reverse DNS lookups

## Entity Structure

### DnsRecord Entity

```csharp
public class DnsRecord
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public string Type { get; set; } // A, AAAA, CNAME, MX, TXT, NS, SRV, etc.
    public string Name { get; set; } // Hostname or subdomain
    public string Value { get; set; } // IP address, hostname, or text value
    public int TTL { get; set; } // Time To Live (default: 3600 seconds)
    public int? Priority { get; set; } // For MX and SRV records
    public int? Weight { get; set; } // For SRV records
    public int? Port { get; set; } // For SRV records
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## API Endpoints

### Base URL: `/api/v1/DnsRecords`

### 1. Get All DNS Records
**GET** `/api/v1/DnsRecords`
- **Authorization**: Admin, Support roles
- **Returns**: List of all DNS records

### 2. Get DNS Record by ID
**GET** `/api/v1/DnsRecords/{id}`
- **Authorization**: Admin, Support, Customer roles
- **Returns**: Single DNS record

### 3. Get DNS Records by Domain ID
**GET** `/api/v1/DnsRecords/domain/{domainId}`
- **Authorization**: Admin, Support, Customer roles
- **Returns**: List of DNS records for a specific domain

### 4. Get DNS Records by Type
**GET** `/api/v1/DnsRecords/type/{type}`
- **Authorization**: Admin, Support roles
- **Parameters**: type (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.)
- **Returns**: List of DNS records of the specified type

### 5. Create DNS Record
**POST** `/api/v1/DnsRecords`
- **Authorization**: Admin, Support, Customer roles
- **Request Body**: CreateDnsRecordDto

```json
{
  "domainId": 1,
  "type": "A",
  "name": "www",
  "value": "192.0.2.1",
  "ttl": 3600,
  "priority": null,
  "weight": null,
  "port": null
}
```

### 6. Update DNS Record
**PUT** `/api/v1/DnsRecords/{id}`
- **Authorization**: Admin, Support, Customer roles
- **Request Body**: UpdateDnsRecordDto

### 7. Delete DNS Record
**DELETE** `/api/v1/DnsRecords/{id}`
- **Authorization**: Admin, Support roles
- **Returns**: 204 No Content on success

## Example API Requests

### Creating an A Record
```http
POST /api/v1/DnsRecords
Content-Type: application/json
Authorization: Bearer {token}

{
  "domainId": 1,
  "type": "A",
  "name": "@",
  "value": "192.0.2.1",
  "ttl": 3600
}
```

### Creating an MX Record
```http
POST /api/v1/DnsRecords
Content-Type: application/json
Authorization: Bearer {token}

{
  "domainId": 1,
  "type": "MX",
  "name": "@",
  "value": "mail.example.com",
  "ttl": 3600,
  "priority": 10
}
```

### Creating an SRV Record
```http
POST /api/v1/DnsRecords
Content-Type: application/json
Authorization: Bearer {token}

{
  "domainId": 1,
  "type": "SRV",
  "name": "_sip._tcp",
  "value": "sipserver.example.com",
  "ttl": 3600,
  "priority": 10,
  "weight": 60,
  "port": 5060
}
```

### Creating a TXT Record (SPF)
```http
POST /api/v1/DnsRecords
Content-Type: application/json
Authorization: Bearer {token}

{
  "domainId": 1,
  "type": "TXT",
  "name": "@",
  "value": "v=spf1 include:_spf.google.com ~all",
  "ttl": 3600
}
```

### Creating a CNAME Record
```http
POST /api/v1/DnsRecords
Content-Type: application/json
Authorization: Bearer {token}

{
  "domainId": 1,
  "type": "CNAME",
  "name": "www",
  "value": "example.com",
  "ttl": 3600
}
```

## Service Layer

The `DnsRecordService` provides the following methods:

- `GetAllDnsRecordsAsync()` - Retrieve all DNS records
- `GetDnsRecordByIdAsync(int id)` - Retrieve a specific DNS record
- `GetDnsRecordsByDomainIdAsync(int domainId)` - Retrieve all DNS records for a domain
- `GetDnsRecordsByTypeAsync(string type)` - Retrieve all DNS records of a specific type
- `CreateDnsRecordAsync(CreateDnsRecordDto)` - Create a new DNS record
- `UpdateDnsRecordAsync(int id, UpdateDnsRecordDto)` - Update an existing DNS record
- `DeleteDnsRecordAsync(int id)` - Delete a DNS record

## Database Migration

To apply the database changes for the enhanced DNS record functionality:

1. Stop the running application
2. Run the migration command:
   ```bash
   cd DR_Admin
   dotnet ef migrations add AddDnsRecordEnhancements --context ApplicationDbContext
   dotnet ef database update
   ```

This will add the following fields to the DnsRecords table:
- Priority (nullable int)
- Weight (nullable int)
- Port (nullable int)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)

And create indexes for improved query performance:
- Index on Type
- Composite index on (DomainId, Type)

## Notes

- The `Type` field is automatically converted to uppercase for consistency
- The default TTL is 3600 seconds (1 hour)
- Priority, Weight, and Port fields are optional and only used for specific record types (MX, SRV)
- The service validates that the domain exists before creating or updating DNS records
- All operations are logged using Serilog for auditing and debugging
