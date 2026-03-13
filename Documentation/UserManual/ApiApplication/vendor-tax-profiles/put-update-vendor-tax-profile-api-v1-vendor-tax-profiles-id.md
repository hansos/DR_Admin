# PUT UpdateVendorTaxProfile

PUT UpdateVendorTaxProfile

## Endpoint

```
PUT /api/v1/vendor-tax-profiles/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateVendorTaxProfileDto](../dtos/update-vendor-tax-profile-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [VendorTaxProfileDto](../dtos/vendor-tax-profile-dto.md) |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)




