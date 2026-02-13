# Email Templates - Quick Reference

## 📁 Template Locations

All templates are in: `Templates/` (solution root)

```
Templates/
├── Layouts/                    # Master templates
│   ├── email.html.master.txt
│   ├── email.text.master.txt
│   └── sms.master.txt
│
├── EmailConfirmation/          # Registration & email change
├── PasswordReset/              # Password recovery
├── OrderActivated/             # Service activation
├── DomainRegistered/           # Domain registration success
└── DomainExpired/              # Domain expiration alert
```

---

## 🎨 Template Types

### Each message type has 3 templates:

1. **email.html.txt** - Rich HTML email (primary)
2. **email.text.txt** - Plain text fallback
3. **sms.txt** - SMS message (under 160 chars)

---

## 🔧 How to Edit Templates

### Option 1: Visual Studio
1. Open solution in Visual Studio
2. Navigate to `Templates/` folder
3. Edit `.txt` files directly
4. Save changes
5. Build project (templates auto-copy to output)

### Option 2: File Explorer
1. Navigate to `C:\Source2\DR_Admin\Templates\`
2. Open template files in any text editor
3. Edit content
4. Save
5. Templates automatically reload (10 min cache)

---

## 📝 Placeholder Syntax

Use `{{PropertyName}}` format:

```html
<p>Hello {{CustomerName}},</p>
<p>Your order {{OrderNumber}} is ready!</p>
<a href="{{ConfirmationUrl}}">Click here</a>
```

**Important**: Placeholder names are case-sensitive!

---

## 📋 Available Placeholders by Template

### EmailConfirmation
- `{{ConfirmationUrl}}` - Email confirmation link
- `{{ExpirationDays}}` - Days until link expires
- `{{Email}}` - User's email address

### PasswordReset
- `{{ResetUrl}}` - Password reset link
- `{{ExpirationHours}}` - Hours until link expires
- `{{Email}}` - User's email address

### OrderActivated
- `{{OrderNumber}}` - Order identifier
- `{{ServiceName}}` - Activated service name
- `{{ActivatedAt}}` - Activation date/time (UTC)
- `{{CustomerPortalUrl}}` - Customer portal link

### DomainRegistered
- `{{DomainName}}` - Registered domain
- `{{RegistrationDate}}` - Registration date
- `{{ExpirationDate}}` - Expiration date
- `{{AutoRenew}}` - Auto-renew status
- `{{CustomerPortalUrl}}` - Portal link

### DomainExpired
- `{{DomainName}}` - Expired domain
- `{{ExpiredAt}}` - Expiration date
- `{{AutoRenewEnabled}}` - Auto-renew status
- `{{RenewUrl}}` - Renewal link

---

## 🎨 Master Template Elements

All email templates inherit these from master:

### HTML Master
- Company logo/header
- Brand colors
- Responsive CSS
- Footer with copyright
- "Don't reply" notice

### Text Master
- Company header
- Content area
- Footer with copyright

### SMS Master
- Company name suffix
- Content area

---

## ✏️ Editing Best Practices

### DO:
✅ Use clear, concise language
✅ Include all necessary information
✅ Test placeholders with real data
✅ Keep SMS under 160 characters
✅ Use professional, friendly tone
✅ Include clear call-to-action

### DON'T:
❌ Remove placeholders without checking code
❌ Add complex logic (use code instead)
❌ Break HTML structure
❌ Use very long URLs in SMS
❌ Change placeholder names without updating model
❌ Remove security warnings

---

## 🚀 Testing Your Changes

### 1. Quick Test (Development)
```csharp
// In your code or unit test
var model = new EmailConfirmationModel
{
    ConfirmationUrl = "https://test.com/confirm",
    ExpirationDays = "3",
    Email = "test@example.com"
};

var html = _messagingService.RenderMessage(
    "EmailConfirmation", 
    MessageChannel.EmailHtml, 
    model
);

// Save to file and open in browser
File.WriteAllText("preview.html", html);
```

### 2. Email Preview
1. Send test email to yourself
2. Check in Gmail, Outlook, Apple Mail
3. Verify all links work
4. Check mobile rendering

### 3. Cache Refresh
Templates cache for 10 minutes. To force reload:
- Restart application
- Or wait 10 minutes
- Or modify cache duration in `TemplateLoader.cs`

---

## 🔄 Adding New Email Templates

### Step 1: Create Folder
```
Templates/YourNewMessageType/
```

### Step 2: Create Template Files (ALL 3 REQUIRED!)
```
Templates/YourNewMessageType/
├── email.html.txt    ← HTML version (rich formatting)
├── email.text.txt    ← Plain text version (accessibility & fallback) ✅ REQUIRED
└── sms.txt           ← SMS version (future use)
```

**Important**: Always create BOTH email.html.txt AND email.text.txt for best deliverability!

### Step 3: Create Model Class
```csharp
// MessagingTemplateLib/Models/YourNewMessageTypeModel.cs
namespace MessagingTemplateLib.Models;

public class YourNewMessageTypeModel
{
    public string PropertyName { get; set; } = string.Empty;
    // Add more properties as needed
}
```

### Step 4: Use in Code (Render BOTH versions!)
```csharp
var model = new YourNewMessageTypeModel
{
    PropertyName = "value"
};

// ✅ ALWAYS render both HTML and text versions
var emailBodyHtml = _messagingService.RenderMessage(
    "YourNewMessageType",
    MessageChannel.EmailHtml,
    model
);

var emailBodyText = _messagingService.RenderMessage(
    "YourNewMessageType",
    MessageChannel.EmailText,
    model
);

// ✅ ALWAYS include both in QueueEmailDto
await _emailQueueService.QueueEmailAsync(new QueueEmailDto
{
    To = recipient,
    Subject = "Your Subject",
    BodyHtml = emailBodyHtml,  // ✅ Required
    BodyText = emailBodyText,  // ✅ Required
    // ... other properties
});
```

**Why both versions?**
- ✅ Better email deliverability (lower spam score)
- ✅ Accessibility for screen readers  
- ✅ Fallback for text-only email clients
- ✅ Industry best practice
```

### Step 4: Use in Code
```csharp
var model = new YourNewMessageTypeModel
{
    PropertyName = "value"
};

var html = _messagingService.RenderMessage(
    "YourNewMessageType",
    MessageChannel.EmailHtml,
    model
);
```

---

## 🎯 Common Customizations

### Change Company Logo
Edit: `Templates/Layouts/email.html.master.txt`
```html
<div class="header">
    <img src="https://yourcompany.com/logo.png" />
    <h1>Your Company Name</h1>
</div>
```

### Change Brand Colors
Edit: `Templates/Layouts/email.html.master.txt`
```html
<style>
    .header { 
        background-color: #YOUR_COLOR; 
    }
    .button { 
        background-color: #YOUR_CTA_COLOR; 
    }
</style>
```

### Change Footer
Edit: `Templates/Layouts/email.html.master.txt`
```html
<div class="footer">
    <p>&copy; 2026 Your Company Name</p>
    <p>Contact: support@yourcompany.com | Phone: 555-1234</p>
</div>
```

---

## 📞 Support Contacts

| Issue | Contact |
|-------|---------|
| Template content/design | Marketing Team |
| Placeholder errors | Development Team |
| Email not sending | Operations Team |
| Template not found | DevOps Team |

---

## 📚 Documentation

- **Full Migration Guide**: `Documentation/Email-Template-Migration-Summary.md`
- **Architecture Details**: `Documentation/Template-System-Architecture.md`
- **All Email Types**: `Documentation/Email-Notification-Endpoints-List.md`
- **File Locations**: `Documentation/Template-System-File-Locations.md`
- **Template README**: `Templates/README.md`

---

**Quick Reference Version**: 1.0  
**Last Updated**: 2026-01-15
