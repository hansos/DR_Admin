# PUT UpdateCustomerTaxProfile

Updates an existing customer tax profile

## Endpoint

```
PUT /api/v1/customer-tax-profiles/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateCustomerTaxProfileDto](../dtos/update-customer-tax-profile-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CustomerTaxProfileDto](../dtos/customer-tax-profile-dto.md) |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




