# POST CreateUnit

Creates a new unit of measurement in the system

## Endpoint

```
POST /api/v1/units
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateUnitDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `UnitDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
