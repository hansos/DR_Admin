# Template System - Quick Location Guide

## ? Where Are the Templates?

### Source Templates (Edit These)
```
C:\Source2\DR_Admin\Templates\
??? Layouts\
?   ??? email.html.master.txt
?   ??? email.text.master.txt
?   ??? sms.master.txt
??? DomainRegistered\
?   ??? email.html.txt
?   ??? email.text.txt
?   ??? sms.txt
??? DomainExpired\
    ??? email.html.txt
    ??? email.text.txt
    ??? sms.txt
```

### Runtime Templates (Auto-Copied During Build)
```
C:\Source2\DR_Admin\DR_Admin\bin\Debug\net10.0\Templates\
(Same structure as above)
```

---

## ? Where Is the Template Engine Code?

### EmailSenderLib Project
```
C:\Source2\DR_Admin\EmailSenderLib\
??? Enums\
?   ??? MessageChannel.cs                    ? EmailHtml, EmailText, Sms
??? Templating\
?   ??? TemplateRenderer.cs                  ? Placeholder engine
?   ??? TemplateLoader.cs                    ? File loading & caching
?   ??? MessagingService.cs                  ? Main API
?   ??? Models\
?       ??? DomainRegisteredModel.cs         ? Data model
?       ??? DomainExpiredModel.cs            ? Data model
??? EmailSenderLib.csproj                    ? Added caching package
```

---

## ? Where Are the Event Handlers (Users of Templates)?

### DR_Admin Project
```
C:\Source2\DR_Admin\DR_Admin\Workflow\Domain\EventHandlers\
??? DomainRegisteredEventHandler.cs          ? Uses templates
??? DomainExpiredEventHandler.cs             ? Uses templates
```

---

## ? Configuration

### DR_Admin.csproj
Added this to automatically copy templates:
```xml
<ItemGroup>
  <None Include="..\Templates\**\*.*" Link="Templates\%(RecursiveDir)%(Filename)%(Extension)">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Program.cs
Added template services to DI:
```csharp
// Email Template System
builder.Services.AddSingleton(sp => new EmailSenderLib.Templating.TemplateLoader(
    sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
    "Templates"
));
builder.Services.AddSingleton<EmailSenderLib.Templating.MessagingService>();
```

---

## ? How to Use

### 1. Edit Templates
Edit files in: `C:\Source2\DR_Admin\Templates\`

### 2. Build Project
Templates automatically copy to: `bin\Debug\net10.0\Templates\`

### 3. Run Application
Application loads templates from: `{AppDirectory}\Templates\`

---

## ? Verified Working

- ? Build successful
- ? Templates copied to output directory
- ? All template files present
- ? Event handlers updated to use templates
- ? Dependency injection configured

---

## ?? Summary

Everything is in place and working! The template system is:

1. **Templates** ? At solution root: `Templates\`
2. **Engine Code** ? In MessagingTemplateLib: `MessagingTemplateLib\Templating\`
3. **Auto-Copy** ? Configured in `DR_Admin.csproj`
4. **DI Setup** ? Configured in `Program.cs`
5. **Usage** ? Event handlers use `MessagingService`

**Just edit templates, build, and run!** ??

