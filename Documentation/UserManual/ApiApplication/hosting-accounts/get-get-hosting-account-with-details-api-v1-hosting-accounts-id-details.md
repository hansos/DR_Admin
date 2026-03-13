# GET GetHostingAccountWithDetails

Retrieves a hosting account with full details including domains, emails, databases, and FTP accounts

## Endpoint

```
GET /api/v1/hosting-accounts/{id}/details
```

## Authorization

Requires authentication. Policy: **Hosting.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [HostingAccountDto](../dtos/hosting-account-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




