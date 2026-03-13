# PUT UpdateServerType

Updates an existing server type's information

## Endpoint

```
PUT /api/v1/server-types/{id}
```

## Authorization

Requires authentication. Policy: **ServerType.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateServerTypeDto](../dtos/update-server-type-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ServerTypeDto](../dtos/server-type-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



