# PUT UpdateCoupon

Updates an existing coupon

## Endpoint

```
PUT /api/v1/coupons/{id}
```

## Authorization

Requires authentication. Policy: **Coupon.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateCouponDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CouponDto` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
