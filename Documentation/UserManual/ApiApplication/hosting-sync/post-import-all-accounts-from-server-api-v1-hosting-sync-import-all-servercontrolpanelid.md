# POST ImportAllAccountsFromServer

Imports all hosting accounts from a server to the database

## Endpoint

```
POST /api/v1/hosting-sync/import-all/{serverControlPanelId}
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `serverControlPanelId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SyncResultDto` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
