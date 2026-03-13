# GET CompareWithServer

Compares a hosting account in the database with its state on the server

## Endpoint

```
GET /api/v1/hosting-sync/compare/{hostingAccountId}
```

## Authorization

Requires authentication. Policy: **Hosting.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [SyncComparisonDto](../dtos/sync-comparison-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




