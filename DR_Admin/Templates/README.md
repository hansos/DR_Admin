# Email Template System

A flexible, text-based templating system for emails and SMS messages with master layout support.

## Overview

The template system separates content from presentation using:
- **Master Templates (Layouts)**: Common branding, headers, footers
- **Message Templates**: Specific content for each message type
- **Placeholders**: `{{PropertyName}}` syntax for dynamic content

## Directory Structure

```
Templates/
??? Layouts/                          # Master templates
?   ??? email.html.master.txt        # HTML email layout
?   ??? email.text.master.txt        # Plain text email layout
?   ??? sms.master.txt               # SMS layout
?
??? DomainRegistered/                 # Message templates
?   ??? email.html.txt
?   ??? email.text.txt
?   ??? sms.txt
?
??? DomainExpired/
?   ??? email.html.txt
?   ??? email.text.txt
?   ??? sms.txt
?
??? [OtherMessageTypes]/
    ??? ...
```

## How It Works

### 1. Master Templates

Master templates contain the common layout and a `{{Content}}` placeholder where message content is injected.

**Example: `Templates/Layouts/email.html.master.txt`**
```html
<html>
<head>
  <style>
    body { font-family: Arial; }
    .header { background: #004080; color: white; }
  </style>
</head>
<body>
  <div class="header">
    <h1>Company Logo</h1>
  </div>
  
  {{Content}}
  
  <footer>© 2026 Company Name</footer>
</body>
</html>
```

### 2. Message Templates

Message templates contain only the specific content for that message type, using placeholders for dynamic data.

**Example: `Templates/DomainRegistered/email.html.txt`**
```html
<h2>Welcome! Your Domain is Ready ??</h2>
<p>Hello,</p>
<p>Your domain <strong>{{DomainName}}</strong> has been successfully registered.</p>
<ul>
  <li>Registration Date: {{RegistrationDate}}</li>
  <li>Expiration Date: {{ExpirationDate}}</li>
</ul>
```

### 3. Rendering Process

1. Load the message template
2. Replace placeholders with model data
3. Load the master template
4. Inject rendered content into `{{Content}}` placeholder
5. Return final rendered message

## Usage

### Setup (Dependency Injection)

```csharp
// In Program.cs or Startup.cs
services.AddMemoryCache();
services.AddSingleton(new TemplateLoader(
    serviceProvider.GetRequiredService<IMemoryCache>(),
    "Templates" // Base path
));
services.AddSingleton<MessagingService>();
```

### Create a Model Class

```csharp
public class DomainRegisteredModel
{
    public string DomainName { get; set; }
    public string RegistrationDate { get; set; }
    public string ExpirationDate { get; set; }
    public string AutoRenew { get; set; }
    public string CustomerPortalUrl { get; set; }
}
```

### Render a Message

```csharp
public class DomainRegisteredEventHandler
{
    private readonly MessagingService _messagingService;
    
    public async Task HandleAsync(DomainRegisteredEvent @event)
    {
        // Create model with data
        var model = new DomainRegisteredModel
        {
            DomainName = @event.DomainName,
            RegistrationDate = @event.OccurredAt.ToString("yyyy-MM-dd"),
            ExpirationDate = @event.ExpirationDate.ToString("yyyy-MM-dd"),
            AutoRenew = @event.AutoRenew ? "Enabled" : "Disabled",
            CustomerPortalUrl = "https://portal.example.com"
        };
        
        // Render HTML email with master template
        var emailBody = _messagingService.RenderMessage(
            "DomainRegistered",          // Message type (folder name)
            MessageChannel.EmailHtml,     // Channel
            model                         // Data model
        );
        
        // Send the email
        await _emailService.QueueEmailAsync(new QueueEmailDto
        {
            To = customer.Email,
            Subject = "Domain Registration Successful",
            BodyHtml = emailBody
        });
    }
}
```

## Adding New Message Types

### Step 1: Create Template Folder

Create a folder under `Templates/` with your message type name (e.g., `InvoiceCreated`).

### Step 2: Create Template Files

Create template files for each channel you support:
- `email.html.txt` - HTML email content
- `email.text.txt` - Plain text email content
- `sms.txt` - SMS content

### Step 3: Create Model Class

```csharp
namespace EmailSenderLib.Templating.Models;

public class InvoiceCreatedModel
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string TotalAmount { get; set; } = string.Empty;
    public string DueDate { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
}
```

### Step 4: Use in Event Handler

```csharp
var model = new InvoiceCreatedModel
{
    InvoiceNumber = invoice.Number,
    CustomerName = customer.Name,
    TotalAmount = invoice.Total.ToString("C"),
    DueDate = invoice.DueDate.ToString("yyyy-MM-dd"),
    PaymentUrl = $"https://portal.example.com/invoices/{invoice.Id}/pay"
};

var emailBody = _messagingService.RenderMessage(
    "InvoiceCreated", 
    MessageChannel.EmailHtml, 
    model
);
```

## Placeholder Syntax

- Use `{{PropertyName}}` format
- Property names are case-sensitive and must match model properties exactly
- Unmatched placeholders remain as-is in the output
- All property values are converted to strings using `.ToString()`

## Caching

Templates are cached in memory for 10 minutes to improve performance:
- First request loads from disk
- Subsequent requests use cached version
- Cache automatically expires and reloads after 10 minutes

## Best Practices

1. **Keep Content Simple**: Templates are text-based, no logic or loops
2. **Format Dates**: Format dates/numbers in your model before passing to templates
3. **Use Descriptive Names**: Model property names should be self-documenting
4. **Test All Channels**: Create templates for HTML, text, and SMS when applicable
5. **Version Control**: Keep templates in source control alongside code
6. **Preview Templates**: Test template rendering before deploying

## Channels

- `MessageChannel.EmailHtml` - Rich HTML emails
- `MessageChannel.EmailText` - Plain text emails (fallback)
- `MessageChannel.Sms` - SMS messages (keep under 160 characters)

## Examples

See the following message types for examples:
- `Templates/DomainRegistered/` - Welcome message with domain details
- `Templates/DomainExpired/` - Urgent notification with action required
