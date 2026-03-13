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
| `processDto` | Body | `ProcessVendorPayoutDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `VendorPayoutDto` |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
