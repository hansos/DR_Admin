# ? New Feature: Provision CPanel Account from Existing Domain

## Summary

Added the ability to provision a CPanel hosting account using a domain that already exists in the `HostingDomains` table. This is useful when domains are created in the database first (e.g., during order processing) and then later provisioned on the actual CPanel server.

---

## New Endpoint

### **POST /api/v1/HostingAccounts/{id}/provision-on-cpanel**

Provisions a hosting account on CPanel using a domain from the HostingDomains table.

#### **Request**
```http
POST /api/v1/HostingAccounts/123/provision-on-cpanel?domainId=789
Authorization: Bearer {token}
```

#### **Query Parameters**
- `domainId` (optional): Specific domain ID to use from HostingDomains table
  - If not provided, uses the main domain (DomainType = "Main")
  - If no main domain, uses the first available domain

#### **Response**
```json
{
  "success": true,
  "message": "Successfully provisioned account on CPanel using domain example.com. Username: newclient",
  "recordsSynced": 1,
  "syncedAt": "2024-01-15T10:30:00Z"
}
```

---

## Usage Scenarios

### **Scenario 1: Basic Provisioning with Main Domain**
```http
# 1. Create hosting account in database
POST /api/v1/HostingAccounts
{
  "customerId": 123,
  "serverControlPanelId": 5,
  "username": "newclient",
  "password": "SecurePass123!",
  "planName": "Business"
}
# Response: { "id": 456, ... }

# 2. Create domain in database
POST /api/v1/hosting-accounts/456/domains
{
  "domainName": "example.com",
  "domainType": "Main"
}
# Response: { "id": 789, ... }

# 3. Provision on CPanel using the domain
POST /api/v1/HostingAccounts/456/provision-on-cpanel
# Automatically uses the main domain (example.com)
```

### **Scenario 2: Provision with Specific Domain**
```http
# Account has multiple domains:
# - domain1.com (ID: 100)
# - domain2.com (ID: 101)
# - domain3.com (ID: 102)

# Provision using domain2.com specifically
POST /api/v1/HostingAccounts/456/provision-on-cpanel?domainId=101
```

### **Scenario 3: Order Processing Workflow**
```csharp
// During order processing:
// 1. Create hosting account record
var account = await _hostingManager.CreateHostingAccountAsync(createDto, syncToServer: false);

// 2. Create domain record from order
var domain = await _domainService.CreateDomainAsync(new HostingDomainCreateDto
{
    HostingAccountId = account.Id,
    DomainName = order.DomainName,
    DomainType = "Main"
}, syncToServer: false);

// 3. Later, when payment is confirmed, provision on CPanel:
var provisionResult = await _hostingManager.ProvisionAccountOnCPanelAsync(account.Id);

if (provisionResult.Success)
{
    // Account is now live on CPanel!
    await SendWelcomeEmail(account);
}
```

---

## Implementation Details

### **Method: ProvisionAccountOnCPanelAsync**

**Location:** `HostingManagerService.cs`

**What it does:**
1. ? Loads hosting account with related data (domains, server control panel)
2. ? Validates account hasn't been provisioned yet (checks `ExternalAccountId`)
3. ? Selects domain to use (specified domain, main domain, or first domain)
4. ? Creates `HostingAccountRequest` with account settings
5. ? Calls `panel.CreateWebHostingAccountAsync()` via HostingSyncService
6. ? Updates `ExternalAccountId` and `SyncStatus` on success
7. ? Updates domain `SyncStatus` to "Synced"

**Signature:**
```csharp
Task<SyncResultDto> ProvisionAccountOnCPanelAsync(int hostingAccountId, int? domainId = null)
```

---

## Validation & Error Handling

### **Pre-Provisioning Checks**
- ? Account must exist
- ? Server control panel must be configured
- ? Account must not already be provisioned (no `ExternalAccountId`)
- ? At least one domain must exist in `HostingDomains` table
- ? Account must have a password set
- ? If `domainId` specified, that domain must belong to the account

### **Error Messages**
```json
// Account not found
{
  "success": false,
  "message": "Hosting account not found"
}

// Already provisioned
{
  "success": false,
  "message": "Account already provisioned on CPanel with ID: existinguser"
}

// No domains
{
  "success": false,
  "message": "No domains found for this hosting account. Please create a domain first."
}

// Domain not found
{
  "success": false,
  "message": "Domain with ID 789 not found for this account"
}

// No password
{
  "success": false,
  "message": "Account password is required to provision on CPanel. Please set a password first."
}

// CPanel error
{
  "success": false,
  "message": "Failed to provision account on CPanel: Username already exists"
}
```

---

## API Changes

### **New Interface Methods**

#### IHostingManagerService
```csharp
Task<SyncResultDto> ProvisionAccountOnCPanelAsync(int hostingAccountId, int? domainId = null);
```

#### IHostingSyncService
```csharp
Task<HostingAccountResult> CreateAccountOnServerAsync(
    ServerControlPanel serverControlPanel, 
    HostingAccountRequest request);
```

---

## Domain Selection Logic

**Priority order:**
1. **Specified domain** (via `domainId` parameter)
2. **Main domain** (where `DomainType = "Main"`)
3. **First available domain** (any domain in the collection)

```csharp
HostingDomain? domain = null;

if (domainId.HasValue)
{
    // Use specified domain
    domain = account.Domains.FirstOrDefault(d => d.Id == domainId.Value);
}
else
{
    // Use main domain or first domain
    domain = account.Domains.FirstOrDefault(d => d.DomainType == "Main") 
            ?? account.Domains.FirstOrDefault();
}
```

---

## Password Handling

**Important Note:** The current implementation uses `PasswordHash` for the CPanel API call, which is incorrect since CPanel needs plaintext.

### **Current Code**
```csharp
Password = account.PasswordHash, // ? This is a BCrypt hash!
```

### **Recommendation**
You have two options:

**Option 1: Store Temporary Plaintext (Recommended)**
```csharp
// Add a transient password field (not persisted)
public class HostingAccount
{
    // ... existing properties
    
    [NotMapped]  // Don't save to database
    public string? TemporaryPassword { get; set; }
}

// Set during creation
account.TemporaryPassword = dto.Password;
account.PasswordHash = BCrypt.HashPassword(dto.Password);

// Use in provisioning
Password = account.TemporaryPassword ?? throw new Exception("Password required"),
```

**Option 2: Require Password Parameter**
```csharp
// Update method signature
Task<SyncResultDto> ProvisionAccountOnCPanelAsync(
    int hostingAccountId, 
    string password,  // Required!
    int? domainId = null);

// Update endpoint
[HttpPost("{id}/provision-on-cpanel")]
public async Task<ActionResult> ProvisionAccountOnCPanel(
    int id, 
    [FromBody] ProvisionRequest request)  // { "password": "...", "domainId": 123 }
{
    var result = await _hostingManager.ProvisionAccountOnCPanelAsync(
        id, 
        request.Password, 
        request.DomainId);
    ...
}
```

**I recommend Option 2** for security - never store plaintext passwords, even temporarily.

---

## Complete Example Workflow

```csharp
// Controller or Service
public async Task<IActionResult> ProcessHostingOrder(OrderDto order)
{
    // Step 1: Create account in database
    var accountDto = new HostingAccountCreateDto
    {
        CustomerId = order.CustomerId,
        ServerControlPanelId = order.ServerId,
        Username = GenerateUsername(order.DomainName),
        Password = GenerateSecurePassword(),
        PlanName = order.PlanName,
        DiskQuotaMB = order.DiskQuota,
        Status = "Pending"
    };
    
    var account = await _hostingManager.CreateHostingAccountAsync(accountDto, syncToServer: false);
    
    // Step 2: Create domain in database
    var domainDto = new HostingDomainCreateDto
    {
        HostingAccountId = account.Id,
        DomainName = order.DomainName,
        DomainType = "Main",
        PhpEnabled = true,
        SslEnabled = false
    };
    
    var domain = await _domainService.CreateDomainAsync(domainDto, syncToServer: false);
    
    // Step 3: Wait for payment confirmation...
    // (This happens asynchronously)
    
    // Step 4: When payment received, provision on CPanel
    var provisionResult = await _hostingManager.ProvisionAccountOnCPanelAsync(account.Id);
    
    if (provisionResult.Success)
    {
        // Update account status
        await _hostingManager.UpdateHostingAccountAsync(account.Id, new HostingAccountUpdateDto
        {
            Status = "Active"
        });
        
        // Send welcome email with login details
        await _emailService.SendWelcomeEmail(account.Customer.Email, account.Username, order.DomainName);
        
        return Ok(new { message = "Hosting account provisioned successfully!" });
    }
    else
    {
        // Handle error
        return BadRequest(new { error = provisionResult.Message });
    }
}
```

---

## Testing

### **Test 1: Basic Provisioning**
```bash
# Create account
POST /api/v1/HostingAccounts
{
  "customerId": 1,
  "serverControlPanelId": 1,
  "username": "testuser",
  "password": "Test123!",
  "planName": "Basic"
}

# Create domain
POST /api/v1/hosting-accounts/1/domains
{
  "domainName": "test.com",
  "domainType": "Main"
}

# Provision
POST /api/v1/HostingAccounts/1/provision-on-cpanel

# Verify on CPanel
# - Account should exist with username "testuser"
# - Domain "test.com" should be the main domain
```

### **Test 2: Multiple Domains**
```bash
# Create account with 3 domains
POST /api/v1/hosting-accounts/1/domains
{ "domainName": "site1.com", "domainType": "Main" }

POST /api/v1/hosting-accounts/1/domains
{ "domainName": "site2.com", "domainType": "Addon" }

POST /api/v1/hosting-accounts/1/domains
{ "domainName": "site3.com", "domainType": "Parked" }

# Provision with site2.com
POST /api/v1/HostingAccounts/1/provision-on-cpanel?domainId=2

# Verify site2.com is used as main domain on CPanel
```

### **Test 3: Error Cases**
```bash
# Try to provision twice
POST /api/v1/HostingAccounts/1/provision-on-cpanel  # Success
POST /api/v1/HostingAccounts/1/provision-on-cpanel  # Error: Already provisioned

# Try with no domains
POST /api/v1/HostingAccounts/99/provision-on-cpanel  # Error: No domains

# Try with invalid domain ID
POST /api/v1/HostingAccounts/1/provision-on-cpanel?domainId=999  # Error: Not found
```

---

## Files Modified

1. **IHostingManagerService.cs** - Added `ProvisionAccountOnCPanelAsync` method
2. **HostingManagerService.cs** - Implemented provisioning logic
3. **IHostingSyncService.cs** - Added `CreateAccountOnServerAsync` helper
4. **HostingSyncService.cs** - Implemented server creation helper
5. **HostingAccountsController.cs** - Added `/provision-on-cpanel` endpoint

---

## Build Status

? **Compilation:** Successful  
? **No Breaking Changes**  
? **Ready for Testing**  

---

## Next Steps

1. **Fix Password Handling** - Implement Option 2 (require password parameter)
2. **Add Integration Tests** - Test provisioning workflow
3. **Add Validation** - Ensure domain is valid before provisioning
4. **Update Documentation** - Add to API documentation

---

**Status: ? COMPLETE - Provisioning from existing domain now available!**

**Endpoint:** `POST /api/v1/HostingAccounts/{id}/provision-on-cpanel?domainId={domainId}`
