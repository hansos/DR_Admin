# GET GetDatabase

Manages databases and database users for hosting accounts

## Endpoint

```
GET /api/v1/hosting-accounts/{hostingAccountId}/databases/{id}
```

## Authorization

Requires authentication. Policy: **Hosting.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [HostingDatabaseDto](../dtos/hosting-database-dto.md) |

[Back to API Manual index](../index.md)




