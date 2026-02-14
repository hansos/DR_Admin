# Email Template Configuration - Complete Guide

## Problem Solved
**Error**: `Template not found: Templates\EmailConfirmation\email.html.txt`  
**Root Cause**: Template path was hard-coded in `Program.cs` instead of being configurable through `appsettings.json`

## Solution Implemented

### 1. Added `TemplatesPath` to `EmailSettings` Class
**File**: `EmailSenderLib\Infrastructure\Settings\EmailSettings.cs`

```csharp
public class EmailSettings
{
    public string Provider { get; set; } = string.Empty;
    public string TemplatesPath { get; set; } = "Templates";  // NEW PROPERTY
    public Smtp? Smtp { get; set; }
    // ... other providers
}
```

**Default Value**: `"Templates"` - Relative path from the application's working directory

### 2. Updated `appsettings.Development.json`
**File**: `DR_Admin\appsettings.Development.json`

```json
{
  "EmailSettings": {
    "Provider": "smtp",
    "TemplatesPath": "Templates",  // NEW CONFIGURATION
    "Smtp": {
      "Host": "email-smtp.us-east-1.amazonaws.com",
      "Port": 587,
      // ... other SMTP settings
    }
  }
}
```

### 3. Updated `Program.cs` to Use Configured Path
**File**: `DR_Admin\Program.cs`

**Before**:
```csharp
builder.Services.AddSingleton(sp => new MessagingTemplateLib.Templating.TemplateLoader(
    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
    "Templates" // Hard-coded path
));
```

**After**:
```csharp
builder.Services.AddSingleton(sp => new MessagingTemplateLib.Templating.TemplateLoader(
    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
    emailSettings.TemplatesPath // Use path from EmailSettings
));
```

## Template Directory Structure

The application expects templates in the following structure:

```
DR_Admin/
└── Templates/
    ├── EmailConfirmation/
    │   ├── email.html.txt   ✅ EXISTS
    │   └── email.text.txt   ✅ EXISTS
    ├── PasswordReset/
    │   ├── email.html.txt   ✅ EXISTS
    │   └── email.text.txt   ✅ EXISTS
    ├── DomainRegistered/
    │   ├── email.html.txt   ✅ EXISTS
    │   ├── email.text.txt   ✅ EXISTS
    │   └── sms.txt          ✅ EXISTS
    ├── DomainExpired/
    │   ├── email.html.txt   ✅ EXISTS
    │   ├── email.text.txt   ✅ EXISTS
    │   └── sms.txt          ✅ EXISTS
    └── Layouts/
        ├── email.html.master.txt  ✅ EXISTS
        ├── email.text.master.txt  ✅ EXISTS
        └── sms.master.txt         ✅ EXISTS
```

## Template File Naming Convention

### Format
`{channel}.txt` where channel is:
- `email.html` - HTML email template
- `email.text` - Plain text email template
- `sms` - SMS template

### Examples
- `Templates/EmailConfirmation/email.html.txt`
- `Templates/PasswordReset/email.text.txt`
- `Templates/DomainRegistered/sms.txt`

## How Templates Are Loaded

### 1. Template Loader Initialization
```csharp
public TemplateLoader(IMemoryCache cache, string templateBasePath = "Templates")
{
    _cache = cache;
    _templateBasePath = templateBasePath;
    _log.Information("TemplateLoader initialized with base path: {BasePath}", _templateBasePath);
}
```

### 2. Template Loading
```csharp
var templateFile = Path.Combine(_templateBasePath, messageType, $"{channel}.txt");
```

**Example**:
- Message Type: `"EmailConfirmation"`
- Channel: `"email.html"`
- Full Path: `Templates/EmailConfirmation/email.html.txt`

### 3. Caching
Templates are cached in memory for 10 minutes to improve performance:
```csharp
entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
```

## Template Models

### EmailConfirmationModel
Used in: `MyAccountService.QueueEmailConfirmationAsync()`

```csharp
var model = new EmailConfirmationModel
{
    ConfirmationUrl = "https://localhost:7155/confirm-email?token=...",
    ExpirationDays = "3",
    Email = "user@example.com"
};
```

**Template Placeholders**:
- `{{ConfirmationUrl}}` - Full URL to confirm email
- `{{ExpirationDays}}` - Number of days until token expires
- `{{Email}}` - User's email address (optional, for reference)

### PasswordResetModel
Used in: `MyAccountService.QueuePasswordResetEmailAsync()`

```csharp
var model = new PasswordResetModel
{
    ResetUrl = "https://localhost:7155/reset-password?token=...",
    ExpirationHours = "24",
    Email = "user@example.com"
};
```

**Template Placeholders**:
- `{{ResetUrl}}` - Full URL to reset password
- `{{ExpirationHours}}` - Number of hours until token expires
- `{{Email}}` - User's email address

## Environment-Specific Configuration

### Development
```json
{
  "EmailSettings": {
    "TemplatesPath": "Templates"
  }
}
```
Relative path - looks in `DR_Admin/Templates/`

### Production (if templates are in a different location)
```json
{
  "EmailSettings": {
    "TemplatesPath": "C:\\EmailTemplates"
  }
}
```
Absolute path - useful if templates are stored separately

### Docker/Container (if needed)
```json
{
  "EmailSettings": {
    "TemplatesPath": "/app/Templates"
  }
}
```

## Working Directory Considerations

The template path is relative to the application's **working directory**, which is typically:
- **Development**: `DR_Admin/bin/Debug/net10.0/`
- **Published**: Where the app is deployed

### Option 1: Copy Templates to Output Directory
Add to `DR_Admin.csproj`:
```xml
<ItemGroup>
  <Content Include="Templates\**\*.txt">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### Option 2: Use Absolute Path
```json
{
  "EmailSettings": {
    "TemplatesPath": "C:\\Source2\\DR_Admin\\DR_Admin\\Templates"
  }
}
```

### Option 3: Use Path Relative to Content Root
Modify `TemplateLoader` to use `IWebHostEnvironment.ContentRootPath`

## Troubleshooting

### Error: Template not found
**Symptoms**: `FileNotFoundException: Template not found: Templates\EmailConfirmation\email.html.txt`

**Solutions**:
1. **Check template path in appsettings**:
   ```json
   "EmailSettings": {
     "TemplatesPath": "Templates"  // Verify this path
   }
   ```

2. **Verify templates exist**:
   ```bash
   Test-Path "DR_Admin\Templates\EmailConfirmation\email.html.txt"
   ```

3. **Check working directory**:
   ```csharp
   _log.Information("Current directory: {Directory}", Directory.GetCurrentDirectory());
   ```

4. **Ensure templates are copied to output**:
   - Add `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` to `.csproj`

5. **Use absolute path** (temporary workaround):
   ```json
   "TemplatesPath": "C:\\Source2\\DR_Admin\\DR_Admin\\Templates"
   ```

### Error: Template has wrong placeholders
**Symptoms**: Email sent but placeholders like `{{ConfirmationUrl}}` are not replaced

**Solutions**:
1. Verify model property names match template placeholders
2. Check template uses `{{PropertyName}}` syntax (double braces)
3. Ensure model is passed to `RenderMessage` correctly

### Error: Templates not updated
**Symptoms**: Changes to templates don't appear in emails

**Solutions**:
1. Wait for cache to expire (10 minutes)
2. Restart application to clear cache
3. Call `TemplateLoader.ClearCache()` (if exposed)

## Best Practices

### ✅ DO
- Define template path in `EmailSettings` section of `appsettings.json`
- Use relative paths for development (e.g., `"Templates"`)
- Use absolute paths for production if needed
- Keep templates in source control
- Version templates alongside code
- Test template rendering before deploying

### ❌ DON'T
- Hard-code template paths in code
- Store templates in database (makes versioning difficult)
- Mix template concerns with other settings
- Forget to copy templates to output directory
- Use different template paths in different environments without documenting

## Verification Steps

### 1. Check Configuration
```bash
# Verify appsettings.Development.json has TemplatesPath
cat DR_Admin\appsettings.Development.json | Select-String "TemplatesPath"
```

### 2. Check Templates Exist
```bash
# List all template files
Get-ChildItem -Path "DR_Admin\Templates" -Recurse -File | Select-Object FullName
```

### 3. Test Template Loading
```bash
# Run the application and check logs
# Look for: "TemplateLoader initialized with base path: Templates"
```

### 4. Test Email Sending
```bash
# Register a new user and check email queue
# Verify confirmation email is queued with rendered template
```

## Related Files
- `EmailSenderLib\Infrastructure\Settings\EmailSettings.cs`
- `DR_Admin\appsettings.Development.json`
- `DR_Admin\Program.cs`
- `MessagingTemplateLib\Templating\TemplateLoader.cs`
- `MessagingTemplateLib\Templating\MessagingService.cs`
- `DR_Admin\Services\MyAccountService.cs`
- All files in `DR_Admin\Templates\`

## Summary

The template path is now properly configured in `EmailSettings` and can be changed per environment through `appsettings.json` files. This follows the principle that "path to template files should be defined in the appsettings.Development.json settings file and EmailSettings class."

**Key Changes**:
1. ✅ Added `TemplatesPath` property to `EmailSettings` class
2. ✅ Added `TemplatesPath` configuration to `appsettings.Development.json`
3. ✅ Updated `Program.cs` to use configured path instead of hard-coded value
4. ✅ All required templates exist in correct directory structure
