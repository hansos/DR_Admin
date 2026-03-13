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
| `dto` | Body | [HostingDatabaseUserCreateDto](../dtos/hosting-database-user-create-dto.md) |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [HostingDatabaseUserDto](../dtos/hosting-database-user-dto.md) |

[Back to API Manual index](../index.md)




