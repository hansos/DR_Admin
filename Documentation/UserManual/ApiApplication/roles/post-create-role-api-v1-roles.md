# POST CreateRole

Creates a new role in the system

## Endpoint

```
POST /api/v1/roles
```

## Authorization

Requires authentication. Policy: **Role.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateRoleDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `RoleDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
