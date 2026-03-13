# GET GetDomain

Manages domains for hosting accounts (main, addon, parked, subdomains)

## Endpoint

```
GET /api/v1/hosting-accounts/{hostingAccountId}/domains/{id}
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
| 200 | OK | [HostingDomainDto](../dtos/hosting-domain-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




