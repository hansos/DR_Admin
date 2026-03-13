# POST SyncEmailAccountsFromServer

POST SyncEmailAccountsFromServer

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/emails/sync
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
| 200 | OK | [SyncResultDto](../dtos/sync-result-dto.md) |

[Back to API Manual index](../index.md)




