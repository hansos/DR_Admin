# Email Template Configuration - Quick Reference

## Error Fixed
```
System.IO.FileNotFoundException: Template not found: Templates\EmailConfirmation\email.html.txt
```

## Solution Summary
Added `TemplatesPath` configuration to `EmailSettings` following the principle:  
**"Path to template files should be defined in appsettings.Development.json and EmailSettings class"**

## Quick Checks

### 1. Is TemplatesPath in EmailSettings class?
```bash
# Check EmailSettings.cs
Select-String -Path "EmailSenderLib\Infrastructure\Settings\EmailSettings.cs" -Pattern "TemplatesPath"
```
✅ Should return: `public string TemplatesPath { get; set; } = "Templates";`

### 2. Is TemplatesPath in appsettings?
```bash
# Check appsettings.Development.json
Select-String -Path "DR_Admin\appsettings.Development.json" -Pattern "TemplatesPath"
```
✅ Should return: `"TemplatesPath": "Templates",`

### 3. Does Program.cs use it?
```bash
# Check Program.cs
Select-String -Path "DR_Admin\Program.cs" -Pattern "emailSettings.TemplatesPath"
```
✅ Should return: `emailSettings.TemplatesPath // Use path from EmailSettings`

### 4. Are templates being copied?
```bash
# Check .csproj
Select-String -Path "DR_Admin\DR_Admin.csproj" -Pattern "Templates"
```
✅ Should include: `<None Include="Templates\**\*.*">`

### 5. Do templates exist?
```bash
# Source templates
Test-Path "DR_Admin\Templates\EmailConfirmation\email.html.txt"

# Output templates (after build)
Test-Path "DR_Admin\bin\Debug\net10.0\Templates\EmailConfirmation\email.html.txt"
```
✅ Both should return: `True`

## Configuration Structure

```json
{
  "EmailSettings": {
    "Provider": "smtp",
    "TemplatesPath": "Templates",  // ← Path configured here
    "Smtp": {
      "Host": "email-smtp.us-east-1.amazonaws.com",
      "Port": 587,
      "Username": "...",
      "Password": "...",
      "EnableSsl": true,
      "FromEmail": "noreply@sorteberg.com",
      "FromName": "DR Admin System"
    }
  }
}
```

## Template Directory
```
DR_Admin/Templates/
├── EmailConfirmation/
│   ├── email.html.txt
│   └── email.text.txt
├── PasswordReset/
│   ├── email.html.txt
│   └── email.text.txt
├── DomainRegistered/
│   ├── email.html.txt
│   ├── email.text.txt
│   └── sms.txt
├── DomainExpired/
│   ├── email.html.txt
│   ├── email.text.txt
│   └── sms.txt
└── Layouts/
    ├── email.html.master.txt
    ├── email.text.master.txt
    └── sms.master.txt
```

## Files Changed

1. `EmailSenderLib\Infrastructure\Settings\EmailSettings.cs` - Added `TemplatesPath` property
2. `DR_Admin\appsettings.Development.json` - Added `"TemplatesPath": "Templates"`
3. `DR_Admin\Program.cs` - Changed from `"Templates"` to `emailSettings.TemplatesPath`
4. `DR_Admin\DR_Admin.csproj` - Fixed copy path from `..\Templates` to `Templates`

## Restart Required
⚠️ Stop debugger and restart application for changes to take effect

## Test Registration
1. Start application
2. POST to `/api/v1/myaccount/register`
3. Check logs for: `"TemplateLoader initialized with base path: Templates"`
4. Verify email queued with rendered template

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Template not found | Check `TemplatesPath` in appsettings.Development.json |
| Templates not copied | Rebuild: `dotnet build DR_Admin\DR_Admin.csproj` |
| Path is wrong | Check working directory vs template path |
| Configuration not loaded | Restart application (hot reload limitation) |

## Documentation

- **TEMPLATE_PATH_FIX_SUMMARY.md** - This fix complete details
- **TEMPLATE_CONFIGURATION_GUIDE.md** - Complete configuration guide
- **EMAIL_CONFIGURATION_GUIDE.md** - Email system configuration
- **CONFIGURATION_UPDATES.md** - AppSettings changes
- **COMPLETE_SUMMARY.md** - Overall feature summary
