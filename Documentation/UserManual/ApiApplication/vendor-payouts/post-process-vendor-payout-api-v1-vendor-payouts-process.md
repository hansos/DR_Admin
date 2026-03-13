# POST ProcessVendorPayout

POST ProcessVendorPayout

## Endpoint

```
POST /api/v1/vendor-payouts/process
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `processDto` | Body | [ProcessVendorPayoutDto](../dtos/process-vendor-payout-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [VendorPayoutDto](../dtos/vendor-payout-dto.md) |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)




