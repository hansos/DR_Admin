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
| `createDto` | Body | `CreateVendorPayoutDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `VendorPayoutDto` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)
