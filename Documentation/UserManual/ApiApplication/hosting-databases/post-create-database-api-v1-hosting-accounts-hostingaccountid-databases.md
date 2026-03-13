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
| `dto` | Body | `HostingDatabaseCreateDto` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `HostingDatabaseDto` |

[Back to API Manual index](../index.md)
