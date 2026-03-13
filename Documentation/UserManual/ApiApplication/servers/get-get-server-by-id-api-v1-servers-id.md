# GET GetServerById

Manages servers including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/servers/{id}
```

## Authorization

Requires authentication. Policy: **Server.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ServerDto](../dtos/server-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




