# POST ValidateTaxId

Validates a customer's tax ID

## Endpoint

```
POST /api/v1/customer-tax-profiles/validate
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `validateDto` | Body | `[ValidateTaxIdDto](../dtos/validate-tax-id-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[TaxIdValidationResultDto](../dtos/tax-id-validation-result-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



