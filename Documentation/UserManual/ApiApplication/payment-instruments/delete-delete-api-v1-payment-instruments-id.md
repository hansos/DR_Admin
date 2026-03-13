# DELETE Delete

DELETE Delete

## Endpoint

```
DELETE /api/v1/payment-instruments/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)
