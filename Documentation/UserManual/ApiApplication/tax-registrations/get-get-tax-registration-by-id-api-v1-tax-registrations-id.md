# GET GetTaxRegistrationById

Manages seller tax registrations used by VAT and TAX reporting.

## Endpoint

```
GET /api/v1/tax-registrations/{id}
```

## Authorization

Requires authentication. Policy: **TaxRegistration.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[TaxRegistrationDto](../dtos/tax-registration-dto.md)` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)



