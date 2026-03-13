# GET GetVendorPayoutById

Manages vendor payouts

## Endpoint

```
GET /api/v1/vendor-payouts/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `VendorPayoutDto` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
