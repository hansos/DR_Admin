# PUT UpdateTaxRegistration

Updates a tax registration.

## Endpoint

```
PUT /api/v1/tax-registrations/{id}
```

## Authorization

Requires authentication. Policy: **TaxRegistration.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | [UpdateTaxRegistrationDto](../dtos/update-tax-registration-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TaxRegistrationDto](../dtos/tax-registration-dto.md) |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)




