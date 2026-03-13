# POST CreateVendorTaxProfile

POST CreateVendorTaxProfile

## Endpoint

```
POST /api/v1/vendor-tax-profiles
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateVendorTaxProfileDto](../dtos/create-vendor-tax-profile-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[VendorTaxProfileDto](../dtos/vendor-tax-profile-dto.md)` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)



