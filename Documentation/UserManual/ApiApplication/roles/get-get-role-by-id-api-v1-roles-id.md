# GET GetRoleById

Manages user roles including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/roles/{id}
```

## Authorization

Requires authentication. Policy: **Role.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[RoleDto](../dtos/role-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



