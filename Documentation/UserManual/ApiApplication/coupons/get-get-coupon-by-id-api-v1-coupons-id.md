# GET GetCouponById

Manages discount coupons

## Endpoint

```
GET /api/v1/coupons/{id}
```

## Authorization

Requires authentication. Policy: **Coupon.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CouponDto](../dtos/coupon-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




