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
| `createDto` | Body | `CreateVendorTaxProfileDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `VendorTaxProfileDto` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)
