# PUT UpdateRole

Updates an existing role's information

## Endpoint

```
PUT /api/v1/roles/{id}
```

## Authorization

Requires authentication. Policy: **Role.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateRoleDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `RoleDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
