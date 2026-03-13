# POST CreateVendorPayout

POST CreateVendorPayout

## Endpoint

```
POST /api/v1/vendor-payouts
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateVendorPayoutDto](../dtos/create-vendor-payout-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [VendorPayoutDto](../dtos/vendor-payout-dto.md) |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)




