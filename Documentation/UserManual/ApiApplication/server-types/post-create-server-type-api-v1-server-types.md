# POST CreateServerType

Creates a new server type in the system

## Endpoint

```
POST /api/v1/server-types
```

## Authorization

Requires authentication. Policy: **ServerType.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateServerTypeDto](../dtos/create-server-type-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[ServerTypeDto](../dtos/server-type-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



