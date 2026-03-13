# PUT UpdateVendorPayout

PUT UpdateVendorPayout

## Endpoint

```
PUT /api/v1/vendor-payouts/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateVendorPayoutDto](../dtos/update-vendor-payout-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[VendorPayoutDto](../dtos/vendor-payout-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



