# POST ValidateCoupon

Retrieves paginated coupon usage entries

## Endpoint

```
POST /api/v1/coupons/validate
```

## Authorization

Requires authentication. Policy: **Coupon.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `validateDto` | Body | `ValidateCouponDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CouponValidationResultDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
