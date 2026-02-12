# Email Template System - Quick Start Guide

## What Was Implemented

Your application now has a **template-based email system** that separates email content from code:

? **Master Templates** - Consistent branding across all emails (HTML, text, SMS)
? **Message Templates** - Individual content templates with placeholders
? **Template Models** - Strongly-typed data models for each message
? **Automatic Rendering** - Template + Data ? Final Email
? **Caching** - Templates cached in memory for performance

## Example: How It Works

### Before (Hardcoded in C# code)
```csharp
private string BuildWelcomeEmailBody(DomainRegisteredEvent @event)
{
    return $@"
Hello,
Your domain {@event.DomainName} has been successfully registered!
Domain Details:
- Domain Name: {@event.DomainName}
- Registration Date: {@event.OccurredAt:yyyy-MM-dd}
...
";
}
```

### After (Template-based)

**1. Create template file**: `Templates/DomainRegistered/email.html.txt`
```html
<h2>Welcome! Your Domain is Ready ??</h2>
<p>Hello,</p>
<p>Your domain <strong>{{DomainName}}</strong> has been successfully registered.</p>
<ul>
    <li>Registration Date: {{RegistrationDate}}</li>
    <li>Expiration Date: {{ExpirationDate}}</li>
</ul>
```

**2. Use in code**:
```csharp
var model = new DomainRegisteredModel
{
    DomainName = @event.DomainName,
    RegistrationDate = @event.OccurredAt.ToString("yyyy-MM-dd"),
    ExpirationDate = @event.ExpirationDate.ToString("yyyy-MM-dd")
};

var emailBody = _messagingService.RenderMessage(
    "DomainRegistered", 
    MessageChannel.EmailHtml, 
    model
);
```

## What Changed in Your Code

### Files Modified:
1. **DomainRegisteredEventHandler.cs** - Now uses templates instead of hardcoded strings
2. **DomainExpiredEventHandler.cs** - Now uses templates instead of hardcoded strings
3. **Program.cs** - Added template service registrations

### Files Created:

#### Core Template Engine:
- `EmailSenderLib/Enums/MessageChannel.cs` - Email/SMS channel types
- `EmailSenderLib/Templating/TemplateRenderer.cs` - Renders templates with placeholders
- `EmailSenderLib/Templating/TemplateLoader.cs` - Loads and caches templates
- `EmailSenderLib/Templating/MessagingService.cs` - High-level service to render messages

#### Template Models:
- `EmailSenderLib/Templating/Models/DomainRegisteredModel.cs`
- `EmailSenderLib/Templating/Models/DomainExpiredModel.cs`

#### Master Templates (Layouts):
- `Templates/Layouts/email.html.master.txt` - HTML email layout with header/footer
- `Templates/Layouts/email.text.master.txt` - Plain text email layout
- `Templates/Layouts/sms.master.txt` - SMS layout

#### Message Templates:
- `Templates/DomainRegistered/email.html.txt`
- `Templates/DomainRegistered/email.text.txt`
- `Templates/DomainRegistered/sms.txt`
- `Templates/DomainExpired/email.html.txt`
- `Templates/DomainExpired/email.text.txt`
- `Templates/DomainExpired/sms.txt`

## Adding a New Email Template

Let's say you want to add a "Password Reset" email:

### Step 1: Create folder structure
```
Templates/
??? PasswordReset/
    ??? email.html.txt
    ??? email.text.txt
    ??? sms.txt
```

### Step 2: Create template content

**Templates/PasswordReset/email.html.txt**:
```html
<h2>Password Reset Request</h2>
<p>Hello {{CustomerName}},</p>
<p>We received a request to reset your password.</p>
<p>Click the button below to reset your password:</p>
<a href="{{ResetLink}}" class="button">Reset Password</a>
<p>This link expires in {{ExpirationMinutes}} minutes.</p>
<p>If you didn't request this, please ignore this email.</p>
```

### Step 3: Create model class

**EmailSenderLib/Templating/Models/PasswordResetModel.cs**:
```csharp
namespace EmailSenderLib.Templating.Models;

public class PasswordResetModel
{
    public string CustomerName { get; set; } = string.Empty;
    public string ResetLink { get; set; } = string.Empty;
    public string ExpirationMinutes { get; set; } = string.Empty;
}
```

### Step 4: Use in your code

```csharp
public class PasswordResetHandler
{
    private readonly MessagingService _messagingService;
    private readonly IEmailQueueService _emailService;
    
    public async Task SendPasswordResetEmail(User user, string resetToken)
    {
        var model = new PasswordResetModel
        {
            CustomerName = user.FullName,
            ResetLink = $"https://portal.example.com/reset?token={resetToken}",
            ExpirationMinutes = "15"
        };
        
        var emailBody = _messagingService.RenderMessage(
            "PasswordReset",
            MessageChannel.EmailHtml,
            model
        );
        
        await _emailService.QueueEmailAsync(new QueueEmailDto
        {
            To = user.Email,
            Subject = "Password Reset Request",
            BodyHtml = emailBody
        });
    }
}
```

## Customizing Master Templates

Want to change the company branding for all emails? Just edit:

**Templates/Layouts/email.html.master.txt**:
```html
<html>
<head>
    <style>
        .header { 
            background-color: #YOUR_BRAND_COLOR; 
            padding: 20px;
        }
        .logo {
            /* Your logo styles */
        }
    </style>
</head>
<body>
    <div class="header">
        <img src="https://yourcompany.com/logo.png" class="logo" />
        <h1>Your Company Name</h1>
    </div>
    
    {{Content}}
    
    <footer>
        <p>&copy; 2026 Your Company</p>
        <p>Contact: support@yourcompany.com</p>
    </footer>
</body>
</html>
```

**All emails automatically use the new design!**

## Template Placeholder Rules

1. Use `{{PropertyName}}` syntax
2. Property names must match your model class properties exactly
3. Property names are **case-sensitive**
4. All values are converted to strings
5. Format dates/numbers in your model before passing to template

## Testing Your Templates

### Option 1: Preview in Browser
1. Render a template with test data
2. Save to a `.html` file
3. Open in browser to preview

```csharp
var model = new DomainRegisteredModel { /* test data */ };
var html = _messagingService.RenderMessage("DomainRegistered", MessageChannel.EmailHtml, model);
File.WriteAllText("preview.html", html);
// Open preview.html in browser
```

### Option 2: Send test email
```csharp
var model = new DomainRegisteredModel
{
    DomainName = "test-domain.com",
    RegistrationDate = "2026-01-15",
    ExpirationDate = "2027-01-15",
    AutoRenew = "Enabled",
    CustomerPortalUrl = "https://portal.example.com"
};

var emailBody = _messagingService.RenderMessage(
    "DomainRegistered", 
    MessageChannel.EmailHtml, 
    model
);

await _emailService.QueueEmailAsync(new QueueEmailDto
{
    To = "your-test-email@example.com",
    Subject = "TEST: Domain Registration",
    BodyHtml = emailBody
});
```

## Benefits

? **Easier to maintain** - Designers can edit HTML without touching C# code
? **Consistent branding** - Master template ensures all emails look the same
? **Better separation of concerns** - Content vs. code
? **Version control friendly** - Track template changes in Git
? **Multi-channel support** - Same template system for HTML, text, and SMS
? **Performance** - Templates cached in memory
? **Type safety** - Strongly-typed models prevent runtime errors

## Next Steps

1. Customize the master templates with your branding
2. Add more message templates as needed
3. Consider moving URLs to configuration (appsettings.json)
4. Add localization support if needed (create separate template folders per language)

## Need Help?

Check `Templates/README.md` for complete documentation.
