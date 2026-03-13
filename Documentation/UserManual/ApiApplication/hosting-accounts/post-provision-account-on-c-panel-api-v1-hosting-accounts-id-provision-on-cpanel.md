# POST ProvisionAccountOnCPanel

Provisions a hosting account on CPanel using a domain from the HostingDomains table

## Endpoint

```
POST /api/v1/hosting-accounts/{id}/provision-on-cpanel
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `domainId` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[SyncResultDto](../dtos/sync-result-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



