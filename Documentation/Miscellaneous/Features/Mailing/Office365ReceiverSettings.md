# Office365 Receiver Settings - Configuration Guide

This guide explains how to configure `EmailReceiverSettings:Office365` for inbound email reading via Microsoft Graph.

## 1. Appsettings structure

Add/update this section in:

- `DR_Admin/appsettings.json`
- `DR_Admin/appsettings.Development.json`

```json
"EmailReceiverSettings": {
  "Provider": "office365",
  "Selection": {
    "EnabledPluginKeys": [ "office365" ],
    "DefaultPluginKey": "office365",
    "FallbackPluginKeys": [],
    "DisabledPluginKeys": []
  },
  "Office365": {
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "MailboxAddress": "shared-inbox@yourdomain.com",
    "AliasRecipient": "alias@yourdomain.com",
    "DefaultFolder": "Inbox",
    "DefaultMaxItems": 25
  }
}
```

## 2. Where to find values

### `TenantId`

- Microsoft Entra admin center (`entra.microsoft.com`)
- `App registrations` -> your app -> `Overview`
- Copy `Directory (tenant) ID`

### `ClientId`

- Same `Overview` page
- Copy `Application (client) ID`

### `ClientSecret`

- `App registrations` -> your app -> `Certificates & secrets`
- Create `New client secret`
- Copy **Value** (shown once)

### `MailboxAddress`

- The mailbox to read from (user or shared mailbox)
- Example: `support@yourdomain.com`

### `AliasRecipient`

- Optional recipient filter
- Only messages addressed to this alias are returned
- Leave empty (`""`) to disable alias filtering

### `DefaultFolder`

- Mail folder in mailbox, usually `Inbox`

### `DefaultMaxItems`

- Max messages per read call when not overridden by query

## 3. Required Microsoft Graph permissions

Use **Application** permissions for client credential flow:

- `Mail.Read` (read messages)
- `Mail.ReadWrite` (required for mark-as-read)

Then click `Grant admin consent`.

## 4. Security recommendations

- Never store real secrets in source control.
- Prefer User Secrets, environment variables, or secure secret storage.
- Rotate `ClientSecret` regularly.
- Restrict app permissions to minimum required.

## 5. Quick validation

After configuration, call:

- `GET /api/v1/EmailReceiver/messages`

Optional filters:

- `folder`
- `unreadOnly`
- `maxItems`
- `receivedAfterUtc`
- `aliasRecipient`

Example:

`GET /api/v1/EmailReceiver/messages?unreadOnly=true&maxItems=10&aliasRecipient=alias@yourdomain.com`

If configuration is invalid, API returns `400 Bad Request` with details.

## 6. Temporary diagnostics endpoint

Use this endpoint to inspect effective receiver configuration and token claims:

- `GET /api/v1/EmailReceiver/diagnostics`

Optional query parameters are the same as the messages endpoint:

- `folder`
- `unreadOnly`
- `maxItems`
- `receivedAfterUtc`
- `aliasRecipient`

Returned diagnostics include:

- effective mailbox and alias filter
- generated Graph messages endpoint
- token tenant id (`tid`)
- token app id (`appid`)
- token audience (`aud`)
- token roles (`roles`)
- token expiry (`exp`)

Example:

`GET /api/v1/EmailReceiver/diagnostics?aliasRecipient=alias@yourdomain.com`

The Office365 receiver also logs diagnostics/failures with mailbox, endpoint, status code and Graph response payload to help troubleshoot `403 ErrorAccessDenied` issues.

## 7. Common access issue: "Not granted for <tenant>"

If `Mail.Read` / `Mail.ReadWrite` shows **Not granted for <tenant>** in Entra, mailbox reads will fail.

### Required state

- Microsoft Graph permissions must be **Application** permissions (not Delegated).
- `Mail.Read` should show **Granted for <tenant>**.
- `Mail.ReadWrite` should show **Granted for <tenant>** if you use mark-as-read.

### Fix steps

1. Open Entra -> `App registrations` -> your app.
2. Go to `API permissions`.
3. Add Microsoft Graph **Application** permissions:
   - `Mail.Read`
   - `Mail.ReadWrite` (optional if not marking messages as read)
4. Click `Grant admin consent for <tenant>`.
5. Verify status changes to **Granted for <tenant>**.
6. Retry:
   - `GET /api/v1/EmailReceiver/diagnostics`
   - `GET /api/v1/EmailReceiver/messages`
