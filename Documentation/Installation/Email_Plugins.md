# Email Plugin Installation

This document describes how to install and configure the email sender plugins available in DR_Admin. Each plugin wraps a third-party email provider and is resolved at runtime through the shared plugin architecture.

---

## Available Plugins

| Plugin Key   | Display Name | Capabilities         | Assembly                       |
|--------------|--------------|----------------------|--------------------------------|
| `smtp`       | SMTP         | send, attachments    | EmailSenderProvider.Smtp       |
| `mailkit`    | MailKit      | send, attachments    | EmailSenderProvider.MailKit    |
| `graphapi`   | Graph API    | send                 | EmailSenderProvider.GraphApi   |
| `exchange`   | Exchange     | send                 | EmailSenderProvider.Exchange   |
| `sendgrid`   | SendGrid     | send, bulk           | EmailSenderProvider.SendGrid   |
| `mailgun`    | Mailgun      | send, bulk           | EmailSenderProvider.Mailgun    |
| `amazonses`  | Amazon SES   | send, bulk           | EmailSenderProvider.AmazonSes  |
| `postmark`   | Postmark     | send                 | EmailSenderProvider.Postmark   |

---

## Plugin Selection Configuration

Add the `EmailSettings` section to `appsettings.json`. The `Selection` block controls which plugins are loaded and how the runtime picks between them.

```json
"EmailSettings": {
  "Provider": "smtp",
  "TemplatesPath": "Templates",
  "Selection": {
    "DefaultPluginKey": "smtp",
    "EnabledPluginKeys": [ "smtp", "sendgrid" ],
    "FallbackPluginKeys": [ "sendgrid" ],
    "DisabledPluginKeys": []
  }
}
```

### Selection Rules

The `EmailSenderFactory` resolves a provider in this order:

1. **Explicit runtime key** — passed by application code at the call site.
2. **`Provider`** — legacy top-level key kept for backward compatibility.
3. **`Selection.DefaultPluginKey`** — preferred plugin key from configuration.
4. **`Selection.FallbackPluginKeys`** — tried sequentially if the selected plugin cannot create a sender.
5. **First available plugin** — deterministic final fallback when no key matches.

If `EnabledPluginKeys` is empty, the runtime derives the list from `Provider`, `DefaultPluginKey`, and `FallbackPluginKeys`. Setting `DisabledPluginKeys` explicitly excludes plugins regardless of other settings.

---

## Provider Configuration

Each enabled plugin requires a matching provider section inside `EmailSettings`. The plugin's `CanCreate` method checks that the section is present before attempting to build a sender.

### SMTP

```json
"Smtp": {
  "Host": "smtp.example.com",
  "Port": 587,
  "Username": "user@example.com",
  "Password": "secret",
  "EnableSsl": true,
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin"
}
```

### MailKit

```json
"MailKit": {
  "Host": "mail.example.com",
  "Port": 587,
  "Username": "user@example.com",
  "Password": "secret",
  "UseSsl": true,
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin"
}
```

### Microsoft Graph API

```json
"GraphApi": {
  "TenantId": "your-tenant-id",
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret",
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin",
  "Scope": "https://graph.microsoft.com/.default"
}
```

### Exchange

```json
"Exchange": {
  "ServerUrl": "https://mail.example.com/EWS/Exchange.asmx",
  "Username": "admin",
  "Password": "secret",
  "Domain": "EXAMPLE",
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin",
  "Version": "Exchange2013_SP1"
}
```

### SendGrid

```json
"SendGrid": {
  "ApiKey": "SG.xxxxxxxxxx",
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin"
}
```

### Mailgun

```json
"Mailgun": {
  "ApiKey": "key-xxxxxxxxxx",
  "Domain": "mg.example.com",
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin",
  "Region": "US"
}
```

### Amazon SES

```json
"AmazonSes": {
  "AccessKeyId": "AKIA...",
  "SecretAccessKey": "secret",
  "Region": "us-east-1",
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin"
}
```

### Postmark

```json
"Postmark": {
  "ServerToken": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "FromEmail": "noreply@example.com",
  "FromName": "DR Admin"
}
```

---

## Startup Registration

Plugins are registered automatically during application startup based on the keys discovered from configuration. The registration logic in `Program.cs`:

1. Collects all unique plugin keys from `EnabledPluginKeys`, `Provider`, `DefaultPluginKey`, and `FallbackPluginKeys`.
2. Resolves each key to its concrete `IEmailSenderPlugin` type via assembly-qualified name.
3. Registers each plugin as a singleton `IEmailSenderPlugin` in the DI container.
4. Registers `EmailSenderFactory` as a singleton that receives all registered plugins and the `EmailSettings`.

No manual service registration is needed. Adding a plugin key to the configuration is sufficient to activate it.

---

## Minimal Working Example

To send email via SMTP with SendGrid as a fallback:

```json
"EmailSettings": {
  "Provider": "smtp",
  "TemplatesPath": "Templates",
  "Selection": {
    "DefaultPluginKey": "smtp",
    "FallbackPluginKeys": [ "sendgrid" ],
    "DisabledPluginKeys": []
  },
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "user@example.com",
    "Password": "secret",
    "EnableSsl": true,
    "FromEmail": "noreply@example.com",
    "FromName": "DR Admin"
  },
  "SendGrid": {
    "ApiKey": "SG.xxxxxxxxxx",
    "FromEmail": "noreply@example.com",
    "FromName": "DR Admin"
  }
}
```

---

## Key Source Files

| File | Purpose |
|------|---------|
| `EmailSenderLib/Infrastructure/Settings/EmailSettings.cs` | Root configuration model |
| `EmailSenderLib/Infrastructure/Settings/EmailPluginSelectionSettings.cs` | Selection and fallback settings |
| `EmailSenderLib/Plugins/Email/IEmailSenderPlugin.cs` | Plugin contract |
| `EmailSenderLib/Plugins/Email/EmailSenderPluginBase.cs` | Abstract base with shared metadata |
| `EmailSenderLib/Factories/EmailSenderFactory.cs` | Runtime plugin resolution and sender creation |
| `PluginLib/Plugins/PluginSelector.cs` | Generic plugin selection with fallback |
| `EmailSenderProviders/EmailSenderProvider.*/Plugins/Email/*Plugin.cs` | Concrete plugin implementations |
