# POST SyncDomainsFromServer

Synchronizes domains from the hosting server to the database

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/domains/sync
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[SyncResultDto](../dtos/sync-result-dto.md)` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



