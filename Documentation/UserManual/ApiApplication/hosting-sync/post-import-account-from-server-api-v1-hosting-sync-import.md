# POST ImportAccountFromServer

Manages synchronization between database and hosting panel servers

## Endpoint

```
POST /api/v1/hosting-sync/import
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `serverControlPanelId` | Query | `int` |
| `externalAccountId` | Query | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[SyncResultDto](../dtos/sync-result-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



