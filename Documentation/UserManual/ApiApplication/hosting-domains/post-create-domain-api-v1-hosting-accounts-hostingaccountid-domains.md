# POST CreateDomain

Creates a new domain for a hosting account

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/domains
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `dto` | Body | [HostingDomainCreateDto](../dtos/hosting-domain-create-dto.md) |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [HostingDomainDto](../dtos/hosting-domain-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




