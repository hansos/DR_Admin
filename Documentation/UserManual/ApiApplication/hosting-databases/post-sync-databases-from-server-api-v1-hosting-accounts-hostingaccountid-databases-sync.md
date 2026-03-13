# POST SyncDatabasesFromServer

POST SyncDatabasesFromServer

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/databases/sync
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
| 200 | OK | `SyncResultDto` |

[Back to API Manual index](../index.md)
