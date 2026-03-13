# GET GetUnitById

Manages units of measurement for services and products

## Endpoint

```
GET /api/v1/units/{id}
```

## Authorization

Requires authentication. Policy: **Unit.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[UnitDto](../dtos/unit-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



