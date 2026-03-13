# GET GetEmailAccount

Manages email accounts for hosting accounts

## Endpoint

```
GET /api/v1/hosting-accounts/{hostingAccountId}/emails/{id}
```

## Authorization

Requires authentication. Policy: **Hosting.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[HostingEmailAccountDto](../dtos/hosting-email-account-dto.md)` |

[Back to API Manual index](../index.md)



