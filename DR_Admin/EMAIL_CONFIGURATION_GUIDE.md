# Email Configuration Guide

## Overview
This guide explains how email-related configuration should be structured in the DR_Admin application, following the principle that configuration should be defined in `appsettings.json` files and mapped to strongly-typed settings classes.

## Configuration Hierarchy

### 1. AppSettings (Frontend URLs)
**Purpose**: Configuration for frontend application URLs used in email links

**File**: `DR_Admin\Infrastructure\Settings\AppSettings.cs`
```csharp
public class AppSettings
{
    public string FrontendBaseUrl { get; set; } = "https://localhost:5001";
    // ... other properties
}
```

**Configuration** (`appsettings.Development.json`):
```json
{
  "AppSettings": {
    "FrontendBaseUrl": "https://localhost:7155"
  }
}
```

**Usage**: Email confirmation links, password reset links, etc.

### 2. EmailSettings (SMTP/Email Provider)
**Purpose**: Configuration for email sending (SMTP, SendGrid, etc.)

**File**: `EmailSenderLib\Infrastructure\Settings\EmailSettings.cs`
```csharp
public class EmailSettings
{
    public string Provider { get; set; } = string.Empty;
    public Smtp? Smtp { get; set; }
    public SendGrid? SendGrid { get; set; }
    // ... other providers
}
```

**Configuration** (`appsettings.Development.json`):
```json
{
  "EmailSettings": {
    "Provider": "smtp",
    "Smtp": {
      "Host": "smtp.example.com",
      "Port": 587,
      "Username": "your-username",
      "Password": "your-password",
      "EnableSsl": true,
      "FromEmail": "noreply@example.com",
      "FromName": "Your App Name"
    }
  }
}
```

**Usage**: Sending emails via SMTP or other providers

### 3. Template Configuration (Future Enhancement)
**Purpose**: Configuration for email template file paths

**Recommended Structure** (`appsettings.Development.json`):
```json
{
  "TemplateSettings": {
    "EmailTemplatesPath": "./Templates/Email",
    "SmsTemplatesPath": "./Templates/SMS",
    "DefaultCulture": "en-US"
  }
}
```

**Recommended Class**:
```csharp
public class TemplateSettings
{
    public string EmailTemplatesPath { get; set; } = "./Templates/Email";
    public string SmsTemplatesPath { get; set; } = "./Templates/SMS";
    public string DefaultCulture { get; set; } = "en-US";
}
```

## Current Email Flow

### 1. User Registration
1. User registers via API
2. `MyAccountService.RegisterAsync()` creates user
3. Generates email confirmation token
4. Calls `QueueEmailConfirmationAsync()` which:
   - Builds URL: `{FrontendBaseUrl}/confirm-email?token={token}`
   - Renders email template using `MessagingService`
   - Queues email via `EmailQueueService`

### 2. Email Confirmation
1. User clicks link in email
2. Frontend page (`confirm-email.html`) extracts token
3. Calls API: `POST /api/v1/myaccount/confirm-email`
4. API confirms email and updates user

### 3. Password Reset
1. User requests password reset
2. `MyAccountService.RequestPasswordResetAsync()` generates token
3. Calls `QueuePasswordResetEmailAsync()` which:
   - Builds URL: `{FrontendBaseUrl}/reset-password?token={token}&email={email}`
   - Renders email template
   - Queues email

## Configuration Best Practices

### ✅ DO
- Define all configuration in `appsettings.json` files
- Use strongly-typed settings classes
- Use dependency injection to access settings
- Provide sensible default values in settings classes
- Use different values per environment (Development, Staging, Production)
- Document configuration in README files

### ❌ DON'T
- Hard-code URLs or paths in service code
- Use magic strings for configuration keys
- Directly access `IConfiguration` for complex settings
- Mix configuration concerns (e.g., SMTP settings in AppSettings)

## Example: Adding Template Path Configuration

If you need to add template path configuration in the future:

### 1. Create Settings Class
```csharp
// DR_Admin\Infrastructure\Settings\TemplateSettings.cs
namespace ISPAdmin.Infrastructure.Settings;

public class TemplateSettings
{
    public string EmailTemplatesPath { get; set; } = "./Templates/Email";
    public string SmsTemplatesPath { get; set; } = "./Templates/SMS";
    public string DefaultCulture { get; set; } = "en-US";
}
```

### 2. Add to appsettings.json
```json
{
  "TemplateSettings": {
    "EmailTemplatesPath": "C:\\Templates\\Email",
    "SmsTemplatesPath": "C:\\Templates\\SMS",
    "DefaultCulture": "en-US"
  }
}
```

### 3. Register in Program.cs
```csharp
var templateSettings = builder.Configuration
    .GetSection("TemplateSettings")
    .Get<TemplateSettings>() ?? new TemplateSettings();
builder.Services.AddSingleton(templateSettings);
```

### 4. Inject in Service
```csharp
public class MyService
{
    private readonly TemplateSettings _templateSettings;
    
    public MyService(TemplateSettings templateSettings)
    {
        _templateSettings = templateSettings;
    }
    
    public void DoSomething()
    {
        var path = _templateSettings.EmailTemplatesPath;
        // ...
    }
}
```

## Environment-Specific Configuration

### Development
```json
{
  "AppSettings": {
    "FrontendBaseUrl": "https://localhost:7155"
  },
  "EmailSettings": {
    "Provider": "smtp",
    "Smtp": {
      "Host": "localhost",
      "Port": 1025,
      "FromEmail": "dev@localhost"
    }
  }
}
```

### Production
```json
{
  "AppSettings": {
    "FrontendBaseUrl": "https://www.yourdomain.com"
  },
  "EmailSettings": {
    "Provider": "smtp",
    "Smtp": {
      "Host": "smtp.yourdomain.com",
      "Port": 587,
      "FromEmail": "noreply@yourdomain.com"
    }
  }
}
```

## Testing Email Configuration

### 1. Local Development (with Papercut)
```json
{
  "EmailSettings": {
    "Provider": "smtp",
    "Smtp": {
      "Host": "localhost",
      "Port": 25,
      "EnableSsl": false
    }
  }
}
```

### 2. Local Development (with Mailhog)
```json
{
  "EmailSettings": {
    "Provider": "smtp",
    "Smtp": {
      "Host": "localhost",
      "Port": 1025,
      "EnableSsl": false
    }
  }
}
```

## Security Considerations

### Sensitive Data
- **DO NOT** commit passwords or API keys to source control
- Use User Secrets for development
- Use environment variables or Azure Key Vault for production

### Using User Secrets (Development)
```bash
dotnet user-secrets set "EmailSettings:Smtp:Password" "your-password"
dotnet user-secrets set "EmailSettings:Smtp:Username" "your-username"
```

### Environment Variables (Production)
```bash
export EmailSettings__Smtp__Password="your-password"
export EmailSettings__Smtp__Username="your-username"
```

## Troubleshooting

### Issue: Email links point to wrong URL
**Solution**: Check `AppSettings:FrontendBaseUrl` in `appsettings.Development.json`

### Issue: Emails not sending
**Solution**: Check `EmailSettings` configuration and SMTP credentials

### Issue: Configuration not loading
**Solution**: 
1. Verify JSON syntax in appsettings files
2. Check that settings are registered in `Program.cs`
3. Restart the application (hot reload may not pick up changes)

## Related Files
- `DR_Admin\Infrastructure\Settings\AppSettings.cs`
- `EmailSenderLib\Infrastructure\Settings\EmailSettings.cs`
- `DR_Admin\Program.cs`
- `DR_Admin\Services\MyAccountService.cs`
- `DR_Admin\appsettings.Development.json`
