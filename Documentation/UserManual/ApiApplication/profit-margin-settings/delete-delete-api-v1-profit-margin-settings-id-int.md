# DELETE Delete

Deletes a profit margin setting.

## Endpoint

```
DELETE /api/v1/profit-margin-settings/{id:int}
```

## Authorization

Requires authentication. Policy: **ProfitMargin.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
