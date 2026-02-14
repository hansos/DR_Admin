# Template Path Configuration - Final Summary

## Problem
```
System.IO.FileNotFoundException: Template not found: Templates\EmailConfirmation\email.html.txt
```

**Error Location**: `MessagingTemplateLib.Templating.TemplateLoader`  
**User's Requirement**: "Path for templates must be defined in EmailSettings"

## Root Causes Identified

1. **Hard-coded Template Path** - Path was hard-coded in `Program.cs` instead of being configurable
2. **Missing Configuration Property** - `EmailSettings` class didn't have a `TemplatesPath` property  
3. **Incorrect Copy Configuration** - `.csproj` was copying from wrong directory (parent `Templates` instead of `DR_Admin\Templates`)

## Complete Solution

### 1. Added `TemplatesPath` to EmailSettings

**File**: `EmailSenderLib\Infrastructure\Settings\EmailSettings.cs`

```csharp
public class EmailSettings
{
    public string Provider { get; set; } = string.Empty;
    public string TemplatesPath { get; set; } = "Templates";  // ← NEW
    public Smtp? Smtp { get; set; }
    // ... other properties
}
```

### 2. Updated Configuration File

**File**: `DR_Admin\appsettings.Development.json`

```json
{
  "EmailSettings": {
    "Provider": "smtp",
    "TemplatesPath": "Templates",  // ← NEW
    "Smtp": {
      "Host": "email-smtp.us-east-1.amazonaws.com",
      "Port": 587,
      "Username": "AKIA5PMBPRK74QRLX2PU",
      "Password": "BC0AWObcwrEwXUceWtr4YWcHMRZHBWWye6x0hmcYTbNj",
      "EnableSsl": true,
      "FromEmail": "noreply@sorteberg.com",
      "FromName": "DR Admin System"
    }
  }
}
```

### 3. Updated TemplateLoader Registration

**File**: `DR_Admin\Program.cs`

**Before**:
```csharp
builder.Services.AddSingleton(sp => new MessagingTemplateLib.Templating.TemplateLoader(
    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
    "Templates" // ← Hard-coded
));
```

**After**:
```csharp
builder.Services.AddSingleton(sp => new MessagingTemplateLib.Templating.TemplateLoader(
    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
    emailSettings.TemplatesPath // ← From configuration
));
```

### 4. Fixed Template Copy Configuration

**File**: `DR_Admin\DR_Admin.csproj`

**Before**:
```xml
<ItemGroup>
  <None Include="..\Templates\**\*.*" Link="Templates\%(RecursiveDir)%(Filename)%(Extension)">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

**After**:
```xml
<ItemGroup>
  <None Include="Templates\**\*.*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

## Template Directory Structure

Templates are located in `DR_Admin\Templates\` and copied to output directory:

```
DR_Admin/
├── Templates/                    ← Source templates
│   ├── EmailConfirmation/
│   │   ├── email.html.txt       ✅ Exists
│   │   └── email.text.txt       ✅ Exists
│   ├── PasswordReset/
│   │   ├── email.html.txt       ✅ Exists
│   │   └── email.text.txt       ✅ Exists
│   ├── DomainRegistered/
│   │   ├── email.html.txt
│   │   ├── email.text.txt
│   │   └── sms.txt
│   ├── DomainExpired/
│   │   ├── email.html.txt
│   │   ├── email.text.txt
│   │   └── sms.txt
│   └── Layouts/
│       ├── email.html.master.txt
│       ├── email.text.master.txt
│       └── sms.master.txt
│
└── bin/Debug/net10.0/
    └── Templates/                ← Copied at build time
        └── (same structure)
```

## Files Modified

| File | Change | Purpose |
|------|--------|---------|
| `EmailSenderLib\Infrastructure\Settings\EmailSettings.cs` | Added `TemplatesPath` property | Enable template path configuration |
| `DR_Admin\appsettings.Development.json` | Added `"TemplatesPath": "Templates"` | Configure template path |
| `DR_Admin\Program.cs` | Use `emailSettings.TemplatesPath` | Read from configuration |
| `DR_Admin\DR_Admin.csproj` | Fixed template copy path | Copy templates from correct directory |

## Configuration Principle Followed

✅ **"Path to template files should be defined in the appsettings.Development.json settings file and EmailSettings class"**

The solution now follows this principle:
1. **EmailSettings Class** - Has `TemplatesPath` property with default value
2. **appsettings.Development.json** - Defines the actual path per environment
3. **Program.cs** - Uses configured value instead of hard-coded value

## How It Works

### 1. Configuration Loading
```csharp
// Load EmailSettings from appsettings
var emailSettings = builder.Configuration
    .GetSection("EmailSettings")
    .Get<EmailSenderLib.Infrastructure.Settings.EmailSettings>()
    ?? new EmailSenderLib.Infrastructure.Settings.EmailSettings();
```

### 2. TemplateLoader Initialization
```csharp
// Pass configured path to TemplateLoader
builder.Services.AddSingleton(sp => new MessagingTemplateLib.Templating.TemplateLoader(
    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
    emailSettings.TemplatesPath  // "Templates" from config
));
```

### 3. Template Loading
```csharp
// TemplateLoader constructs path
var templateFile = Path.Combine(
    _templateBasePath,    // "Templates"
    messageType,          // "EmailConfirmation"  
    $"{channel}.txt"      // "email.html.txt"
);
// Result: "Templates/EmailConfirmation/email.html.txt"
```

### 4. Template Lookup
```
Working Directory: DR_Admin/bin/Debug/net10.0/
Template Path: Templates/EmailConfirmation/email.html.txt
Full Path: DR_Admin/bin/Debug/net10.0/Templates/EmailConfirmation/email.html.txt ✅
```

## Environment-Specific Configuration

### Development (`appsettings.Development.json`)
```json
{
  "EmailSettings": {
    "TemplatesPath": "Templates"
  }
}
```
Relative path - templates copied to output directory

### Production (`appsettings.Production.json`)
```json
{
  "EmailSettings": {
    "TemplatesPath": "/var/app/templates"
  }
}
```
Absolute path - templates in fixed location

### Docker Container
```json
{
  "EmailSettings": {
    "TemplatesPath": "/app/Templates"
  }
}
```
Container path - templates mounted as volume

## Testing

### Verify Configuration
```bash
# Check appsettings has TemplatesPath
cat DR_Admin\appsettings.Development.json | Select-String "TemplatesPath"
```

### Verify Templates Exist
```bash
# Source templates
Test-Path "DR_Admin\Templates\EmailConfirmation\email.html.txt"

# Output templates (after build)
Test-Path "DR_Admin\bin\Debug\net10.0\Templates\EmailConfirmation\email.html.txt"
```

### Verify Copy Works
```bash
# Rebuild and check output
dotnet build DR_Admin\DR_Admin.csproj
Get-ChildItem -Path "DR_Admin\bin\Debug\net10.0\Templates" -Recurse
```

### Test Registration Flow
1. Start application
2. Register new user
3. Check logs for template loading:
   ```
   [INFO] TemplateLoader initialized with base path: Templates
   [INFO] Loading template from: Templates/EmailConfirmation/email.html.txt
   [INFO] Email confirmation queued for user@example.com
   ```

## Documentation Created

1. **`TEMPLATE_CONFIGURATION_GUIDE.md`** - Complete guide for template configuration
2. **`CONFIGURATION_UPDATES.md`** - AppSettings and FrontendBaseUrl changes
3. **`EMAIL_CONFIGURATION_GUIDE.md`** - Email configuration best practices
4. **`COMPLETE_SUMMARY.md`** - Overall email confirmation feature summary

## Benefits

### ✅ Configurability
- Template path can be changed per environment
- No code changes needed to change path
- Supports relative and absolute paths

### ✅ Consistency
- Follows same pattern as other settings (DbSettings, RegistrarSettings, etc.)
- All configuration in `appsettings.json` files
- Strongly-typed through `EmailSettings` class

### ✅ Flexibility
- Development: Templates in project directory
- Production: Templates in deployment directory
- Docker: Templates as mounted volume
- Testing: Mock templates in temp directory

### ✅ Maintainability
- Clear documentation
- Self-documenting default values
- Easy to troubleshoot (check config first)

## Next Steps

1. **Restart Application** - Hot reload cannot handle field renames in MyAccountService
2. **Test Registration** - Register a new user and verify email is sent
3. **Check Logs** - Verify templates are loaded from correct path
4. **Verify Email** - Check that email contains properly rendered templates

## Related Changes

This work builds on previous changes:
- Email confirmation API refactoring (removed email parameter)
- AppSettings refactoring (added FrontendBaseUrl)
- Frontend confirmation page (confirm-email.html)

All changes follow the principle: **Configuration should be in appsettings files and mapped to strongly-typed settings classes**.

## Verification Checklist

- ✅ `EmailSettings` class has `TemplatesPath` property
- ✅ `appsettings.Development.json` defines `TemplatesPath`
- ✅ `Program.cs` uses configured path
- ✅ `.csproj` copies templates to output directory
- ✅ Templates exist in source directory
- ✅ Templates copied to output directory
- ✅ Build succeeds
- ✅ Documentation complete

## Restart Required

⚠️ **Important**: The application must be restarted for these changes to take effect:
- `MyAccountService` field rename (`_configuration` → `_appSettings`)
- `EmailSettings` new property
- Template path configuration
- `.csproj` changes

Stop the debugger and restart the application for changes to apply.
