# DELETE DeleteDatabaseUser

DELETE DeleteDatabaseUser

## Endpoint

```
DELETE /api/v1/hosting-accounts/{hostingAccountId}/databases/{databaseId}/users/{userId}
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `databaseId` | Route | `int` |
| `userId` | Route | `int` |
| `deleteFromServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |

[Back to API Manual index](../index.md)
