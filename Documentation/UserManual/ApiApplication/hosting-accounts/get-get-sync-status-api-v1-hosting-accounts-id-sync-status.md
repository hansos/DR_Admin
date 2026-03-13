# GET GetSyncStatus

Gets sync status for a hosting account

## Endpoint

```
GET /api/v1/hosting-accounts/{id}/sync-status
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
| 200 | OK | [SyncStatusDto](../dtos/sync-status-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




