# PUT UpdateHostingAccount

Updates an existing hosting account

## Endpoint

```
PUT /api/v1/hosting-accounts/{id}
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | `[HostingAccountUpdateDto](../dtos/hosting-account-update-dto.md)` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[HostingAccountDto](../dtos/hosting-account-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



