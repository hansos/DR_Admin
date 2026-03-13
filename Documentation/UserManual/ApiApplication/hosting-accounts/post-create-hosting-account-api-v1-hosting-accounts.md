# POST CreateHostingAccount

Retrieves all hosting accounts for a specific customer

## Endpoint

```
POST /api/v1/hosting-accounts
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `[HostingAccountCreateDto](../dtos/hosting-account-create-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[HostingAccountDto](../dtos/hosting-account-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



