# DELETE DeleteTaxRegistration

Deletes a tax registration.

## Endpoint

```
DELETE /api/v1/tax-registrations/{id}
```

## Authorization

Requires authentication. Policy: **TaxRegistration.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
