# GET GetServerTypeById

Manages server types including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/server-types/{id}
```

## Authorization

Requires authentication. Policy: **ServerType.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ServerTypeDto](../dtos/server-type-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



