# Email Template System - File Locations

## Complete File Structure

```
C:\Source2\DR_Admin\                          ? Solution Root
?
??? Templates\                                ? Template files (solution root)
?   ??? README.md
?   ?
?   ??? Layouts\                              ? Master templates
?   ?   ??? email.html.master.txt
?   ?   ??? email.text.master.txt
?   ?   ??? sms.master.txt
?   ?
?   ??? DomainRegistered\                     ? Domain registration templates
?   ?   ??? email.html.txt
?   ?   ??? email.text.txt
?   ?   ??? sms.txt
?   ?
?   ??? DomainExpired\                        ? Domain expiration templates
?       ??? email.html.txt
?       ??? email.text.txt
?       ??? sms.txt
?
??? MessagingTemplateLib\                     ? Template engine code (NEW!)
?   ??? MessageChannel.cs                     ? Channel types (EmailHtml, EmailText, Sms)
?   ?
?   ??? Templating\
?   ?   ??? TemplateRenderer.cs               ? Placeholder replacement engine
?   ?   ??? TemplateLoader.cs                 ? File loading & caching
?   ?   ??? MessagingService.cs               ? High-level rendering service
?   ?
?   ??? Models\                               ? Template data models
?   ?   ??? DomainRegisteredModel.cs
?   ?   ??? DomainExpiredModel.cs
?   ?
?   ??? MessagingTemplateLib.csproj           ? Added Serilog & Caching packages
?
??? EmailSenderLib.Tests\                     ? Unit tests
?   ??? TemplateSystemTests.cs
?   ??? EmailSenderLib.Tests.csproj
?
??? DR_Admin\                                 ? Main application
?   ??? Program.cs                            ? DI registration for template services
?   ?
?   ??? Workflow\
?   ?   ??? Domain\
?   ?       ??? EventHandlers\
?   ?           ??? DomainRegisteredEventHandler.cs   ? Uses MessagingService
?   ?           ??? DomainExpiredEventHandler.cs      ? Uses MessagingService
?   ?
?   ??? DR_Admin.csproj                       ? References MessagingTemplateLib
?
??? Documentation\                            ? Documentation
    ??? Email-Template-System-QuickStart.md
    ??? Template-System-Implementation-Summary.md
    ??? Template-System-Deployment-Checklist.md
    ??? Template-System-Architecture.md
    ??? Template-System-File-Locations.md     ? This file
```

---

## Template File Locations (Detailed)

### Solution Root Level
Templates folder is at the **solution root** (same level as DR_Admin, MessagingTemplateLib, etc.):

```
C:\Source2\DR_Admin\Templates\
```

**Why here?**
- Shared resource across all projects
- Easy to find and edit
- Version controlled with the solution
- Copied to output directory during build

### At Runtime (Output Directory)
When you build/run the application, templates are copied to:

```
C:\Source2\DR_Admin\DR_Admin\bin\Debug\net10.0\Templates\
```

Or in Release mode:
```
C:\Source2\DR_Admin\DR_Admin\bin\Release\net10.0\Templates\
```

**How?**
The `DR_Admin.csproj` has this configuration:
```xml
<ItemGroup>
  <None Include="..\Templates\**\*.*" Link="Templates\%(RecursiveDir)%(Filename)%(Extension)">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

This ensures templates are always available at runtime!

---

## Code File Locations

### Template Engine (MessagingTemplateLib) ? NEW!

All template engine code is now in the **MessagingTemplateLib** project:

| File | Location | Purpose |
|------|----------|---------|
| MessageChannel.cs | `MessagingTemplateLib\` | Enum for channel types |
| TemplateRenderer.cs | `MessagingTemplateLib\Templating\` | Placeholder replacement |
| TemplateLoader.cs | `MessagingTemplateLib\Templating\` | File I/O and caching |
| MessagingService.cs | `MessagingTemplateLib\Templating\` | High-level API |
| DomainRegisteredModel.cs | `MessagingTemplateLib\Models\` | Data model |
| DomainExpiredModel.cs | `MessagingTemplateLib\Models\` | Data model |

**Why in MessagingTemplateLib?** ?
- ? Dedicated project for messaging templates
- ? Separation of concerns (EmailSenderLib = email sending, MessagingTemplateLib = templating)
- ? Reusable across multiple messaging types (Email, SMS, Push, etc.)
- ? Clear architectural boundaries

### Event Handlers (DR_Admin)

Event handlers that USE the template system:

| File | Location | What Changed |
|------|----------|--------------|
| DomainRegisteredEventHandler.cs | `DR_Admin\Workflow\Domain\EventHandlers\` | Now uses MessagingTemplateLib |
| DomainExpiredEventHandler.cs | `DR_Admin\Workflow\Domain\EventHandlers\` | Now uses MessagingTemplateLib |


---

## How to Verify Templates Are Included

### 1. Check in Visual Studio Solution Explorer

In Visual Studio:
1. Open Solution Explorer
2. Look for `Templates` folder at solution root
3. You should see:
   ```
   Solution 'DR_Admin'
   ??? Templates
   ?   ??? Layouts
   ?   ??? DomainRegistered
   ?   ??? DomainExpired
   ??? DR_Admin
   ??? EmailSenderLib
   ??? ...
   ```

**If you don't see it:**
- The folder exists on disk, but might not show in Solution Explorer
- This is OK! The build configuration will still copy it
- You can add it manually: Right-click solution ? Add ? Existing Item ? Select Templates folder

### 2. Check After Build

After building the project:

```powershell
# Check if templates are in output directory
Get-ChildItem "DR_Admin\bin\Debug\net10.0\Templates\" -Recurse
```

You should see all template files copied to the output directory.

### 3. Check at Runtime

When the application runs, it looks for templates at:
```
{ApplicationDirectory}\Templates\
```

The `TemplateLoader` uses this path:
```csharp
new TemplateLoader(cache, "Templates")
```

---

## Adding New Templates

### Where to Add

Add new template files to the **solution root Templates folder**:

```
C:\Source2\DR_Admin\Templates\YourNewMessageType\
```

### Example: Adding "PasswordReset" Template

1. **Create folder**:
   ```
   C:\Source2\DR_Admin\Templates\PasswordReset\
   ```

2. **Add template files**:
   ```
   C:\Source2\DR_Admin\Templates\PasswordReset\email.html.txt
   C:\Source2\DR_Admin\Templates\PasswordReset\email.text.txt
   C:\Source2\DR_Admin\Templates\PasswordReset\sms.txt
   ```

3. **Create model** (in EmailSenderLib):
   ```
   C:\Source2\DR_Admin\EmailSenderLib\Templating\Models\PasswordResetModel.cs
   ```

4. **Build** - Templates automatically copied to output directory!

**No project file changes needed!** The `**\*.*` wildcard in DR_Admin.csproj automatically includes new templates.

---

## Deployment Considerations

### Development
Templates are at: `C:\Source2\DR_Admin\Templates\`

### After Build
Templates copied to: `DR_Admin\bin\{Configuration}\net10.0\Templates\`

### Production Deployment

**Option 1: Include in publish**
```powershell
dotnet publish -c Release
```
Templates will be in: `publish\Templates\`

**Option 2: Manual deployment**
Copy Templates folder to production server:
```
/var/www/yourapp/Templates/
```

**Option 3: Docker**
In Dockerfile:
```dockerfile
COPY Templates /app/Templates
```

### Verify on Production Server
```bash
ls -la /path/to/app/Templates/
ls -la /path/to/app/Templates/Layouts/
ls -la /path/to/app/Templates/DomainRegistered/
```

All template files should be present and readable!

---

## Common Issues

### Issue: "Template not found" at runtime

**Cause**: Templates not copied to output directory

**Solution**:
1. Verify `DR_Admin.csproj` has the `<None Include="..\Templates\**\*.*">` configuration
2. Rebuild the project
3. Check `bin\Debug\net10.0\Templates\` folder exists

### Issue: Templates not showing in Solution Explorer

**Cause**: Templates folder not added to solution

**Solution**:
This is cosmetic only - templates will still work! But if you want to see them in Solution Explorer:
1. Right-click on solution
2. Add ? Existing Item
3. Select Templates folder
4. Or just edit files directly in your file explorer/editor

### Issue: Changes to templates not reflected

**Cause**: Cached templates (10 minute cache)

**Solution**:
1. Restart application (clears cache)
2. Or wait 10 minutes for cache to expire
3. Or reduce cache duration in `TemplateLoader.cs`

### Issue: Templates not deployed to production

**Cause**: Forgot to copy Templates folder

**Solution**:
Include Templates in deployment:
```powershell
# Include in publish
dotnet publish --include-templates

# Or manually copy
Copy-Item -Path "Templates" -Destination "publish/Templates" -Recurse
```

---

## Quick Reference

| What | Where |
|------|-------|
| Template Files | `C:\Source2\DR_Admin\Templates\` |
| Template Engine Code | `EmailSenderLib\Templating\` |
| Template Models | `EmailSenderLib\Templating\Models\` |
| Event Handlers (users) | `DR_Admin\Workflow\Domain\EventHandlers\` |
| DI Registration | `DR_Admin\Program.cs` |
| Documentation | `Documentation\` |
| Unit Tests | `EmailSenderLib.Tests\TemplateSystemTests.cs` |
| Output Directory (runtime) | `DR_Admin\bin\Debug\net10.0\Templates\` |

---

## Summary

? **Templates** are at solution root: `C:\Source2\DR_Admin\Templates\`  
? **Template engine code** is in: `EmailSenderLib\Templating\`  
? **Templates are automatically copied** to output directory on build  
? **No manual steps needed** - everything is configured!  

Just build and run! ??
