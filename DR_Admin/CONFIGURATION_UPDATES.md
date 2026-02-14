# Configuration Updates - AppSettings and Frontend Base URL

## Summary
Refactored email confirmation and password reset URL generation to use strongly-typed `AppSettings` configuration instead of directly accessing `IConfiguration`.

## Changes Made

### 1. Updated `AppSettings` Class
**File**: `DR_Admin\Infrastructure\Settings\AppSettings.cs`

Added `FrontendBaseUrl` property to the `AppSettings` class:
```csharp
public class AppSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
    public string FrontendBaseUrl { get; set; } = "https://localhost:5001";  // NEW
    public DbSettings DbSettings { get; set; } = new();
    public EmailSettings? MailSettings { get; set; }
}
```

### 2. Updated Configuration File
**File**: `DR_Admin\appsettings.Development.json`

Changed from:
```json
"AppSettings": {
  "BaseUrl": "https://localhost:7201"
}
```

To:
```json
"AppSettings": {
  "FrontendBaseUrl": "https://localhost:7155"
}
```

**Note**: The URL changed from `7201` (API port) to `7155` (Frontend Demo port) since this is the base URL for the frontend application, not the API.

### 3. Updated Program.cs
**File**: `DR_Admin\Program.cs`

Updated the `AppSettings` initialization to read the `FrontendBaseUrl` from configuration:
```csharp
var appSettings = new AppSettings
{
    DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
    FrontendBaseUrl = builder.Configuration["AppSettings:FrontendBaseUrl"] ?? "https://localhost:5001",  // NEW
    DbSettings = builder.Configuration.GetSection("DbSettings").Get<DbSettings>() ?? new DbSettings()
};
```

### 4. Updated MyAccountService
**File**: `DR_Admin\Services\MyAccountService.cs`

#### Changes:
1. **Added using statement**:
   ```csharp
   using ISPAdmin.Infrastructure.Settings;
   ```

2. **Replaced `IConfiguration` with `AppSettings`**:
   ```csharp
   // Before
   private readonly IConfiguration _configuration;
   
   // After
   private readonly AppSettings _appSettings;
   ```

3. **Updated constructor**:
   ```csharp
   public MyAccountService(
       ApplicationDbContext context, 
       AppSettings appSettings,  // Changed from IConfiguration
       IEmailQueueService emailQueueService,
       MessagingService messagingService)
   {
       _context = context;
       _appSettings = appSettings;  // Changed from _configuration
       _emailQueueService = emailQueueService;
       _messagingService = messagingService;
   }
   ```

4. **Updated `QueueEmailConfirmationAsync` method**:
   ```csharp
   // Before
   var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost";
   var confirmationUrl = $"{baseUrl}/confirm-email?token={Uri.EscapeDataString(token)}";
   
   // After
   var confirmationUrl = $"{_appSettings.FrontendBaseUrl}/confirm-email?token={Uri.EscapeDataString(token)}";
   ```

5. **Updated `QueuePasswordResetEmailAsync` method**:
   ```csharp
   // Before
   var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost";
   var resetUrl = $"{baseUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
   
   // After
   var resetUrl = $"{_appSettings.FrontendBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
   ```

## Benefits

### 1. **Strong Typing**
- Configuration values are now strongly-typed
- Compile-time checking for property names
- IntelliSense support

### 2. **Centralized Configuration**
- All app settings in one place (`AppSettings` class)
- Easy to find and modify
- Self-documenting with default values

### 3. **Testability**
- Easier to mock in unit tests
- No need to mock `IConfiguration`
- Can create test instances of `AppSettings` directly

### 4. **Consistency**
- Follows the same pattern as other settings classes (DbSettings, EmailSettings, etc.)
- Consistent with the project's architecture

### 5. **Clarity**
- Clear separation: `FrontendBaseUrl` is for frontend links in emails
- API URL (port 7201) vs Frontend URL (port 7155) distinction is now explicit

## Environment-Specific Configuration

You can override `FrontendBaseUrl` in different environments:

**Development** (`appsettings.Development.json`):
```json
"AppSettings": {
  "FrontendBaseUrl": "https://localhost:7155"
}
```

**Production** (`appsettings.Production.json`):
```json
"AppSettings": {
  "FrontendBaseUrl": "https://www.yourdomain.com"
}
```

## URL Generation Examples

### Email Confirmation
Generated URL: `https://localhost:7155/confirm-email?token=ABC123...`

### Password Reset
Generated URL: `https://localhost:7155/reset-password?token=XYZ789...&email=user@example.com`

## Testing

To test the configuration:
1. Ensure `appsettings.Development.json` has the correct `FrontendBaseUrl`
2. Register a new user
3. Check the email queue for the confirmation email
4. Verify the confirmation URL uses the configured frontend base URL
5. Click the link to test the email confirmation flow

## Notes

- The application must be restarted for these changes to take effect (hot reload cannot handle field renames)
- Make sure the frontend application is running on the configured port
- The default value in `AppSettings.cs` (`https://localhost:5001`) serves as a fallback
- Consider using environment variables in production for sensitive URLs
