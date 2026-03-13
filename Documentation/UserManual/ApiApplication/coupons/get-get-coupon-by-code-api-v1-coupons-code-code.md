# GET GetCouponByCode

Retrieves a coupon by code

## Endpoint

```
GET /api/v1/coupons/code/{code}
```

## Authorization

Requires authentication. Policy: **Coupon.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `code` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CouponDto](../dtos/coupon-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




