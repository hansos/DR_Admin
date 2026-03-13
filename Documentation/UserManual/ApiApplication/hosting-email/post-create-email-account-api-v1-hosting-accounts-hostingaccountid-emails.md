# POST CreateEmailAccount

POST CreateEmailAccount

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/emails
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `dto` | Body | `HostingEmailAccountCreateDto` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `HostingEmailAccountDto` |

[Back to API Manual index](../index.md)
