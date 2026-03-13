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
| `updateDto` | Body | `UpdateVendorTaxProfileDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `VendorTaxProfileDto` |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
