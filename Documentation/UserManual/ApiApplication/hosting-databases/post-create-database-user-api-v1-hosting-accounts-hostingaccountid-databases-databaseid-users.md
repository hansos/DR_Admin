# POST CreateDatabaseUser

POST CreateDatabaseUser

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/databases/{databaseId}/users
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `databaseId` | Route | `int` |
| `dto` | Body | `HostingDatabaseUserCreateDto` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `HostingDatabaseUserDto` |

[Back to API Manual index](../index.md)
