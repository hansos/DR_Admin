# POST CreateTaxRegistration

Creates a tax registration.

## Endpoint

```
POST /api/v1/tax-registrations
```

## Authorization

Requires authentication. Policy: **TaxRegistration.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CreateTaxRegistrationDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `TaxRegistrationDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
