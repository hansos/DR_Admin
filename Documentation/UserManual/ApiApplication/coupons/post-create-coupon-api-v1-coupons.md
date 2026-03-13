# POST CreateCoupon

Creates a new coupon

## Endpoint

```
POST /api/v1/coupons
```

## Authorization

Requires authentication. Policy: **Coupon.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateCouponDto](../dtos/create-coupon-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [CouponDto](../dtos/coupon-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




