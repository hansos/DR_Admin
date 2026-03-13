# GET GetVendorPayoutSummaryByVendorId

GET GetVendorPayoutSummaryByVendorId

## Endpoint

```
GET /api/v1/vendor-payouts/summary/vendor/{vendorId}
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
| 200 | OK | `[VendorPayoutSummaryDto](../dtos/vendor-payout-summary-dto.md)` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



