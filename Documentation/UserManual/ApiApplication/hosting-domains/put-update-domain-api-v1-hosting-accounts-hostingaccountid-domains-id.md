# PUT UpdateDomain

Updates a domain

## Endpoint

```
PUT /api/v1/hosting-accounts/{hostingAccountId}/domains/{id}
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `id` | Route | `int` |
| `dto` | Body | `HostingDomainUpdateDto` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `HostingDomainDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
