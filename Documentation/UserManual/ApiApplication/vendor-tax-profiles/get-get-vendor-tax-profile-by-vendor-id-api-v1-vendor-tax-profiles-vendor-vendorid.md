# GET GetVendorTaxProfileByVendorId

GET GetVendorTaxProfileByVendorId

## Endpoint

```
GET /api/v1/vendor-tax-profiles/vendor/{vendorId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `vendorId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[VendorTaxProfileDto](../dtos/vendor-tax-profile-dto.md)` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



