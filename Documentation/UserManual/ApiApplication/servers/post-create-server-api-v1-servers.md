# POST CreateServer

Creates a new server in the system

## Endpoint

```
POST /api/v1/servers
```

## Authorization

Requires authentication. Policy: **Server.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateServerDto](../dtos/create-server-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[ServerDto](../dtos/server-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



