# POST CreateDatabase

POST CreateDatabase

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/databases
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `dto` | Body | [HostingDatabaseCreateDto](../dtos/hosting-database-create-dto.md) |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [HostingDatabaseDto](../dtos/hosting-database-dto.md) |

[Back to API Manual index](../index.md)




