# CouponValidationResultDto

Data transfer object representing coupon validation result

## Source

`DR_Admin/DTOs/CouponValidationResultDto.cs`

## TypeScript Interface

```ts
export interface CouponValidationResultDto {
  isValid: boolean;
  message: string;
  discountAmount: number;
  coupon: CouponDto | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `IsValid` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `DiscountAmount` | `decimal` | `number` |
| `Coupon` | `CouponDto?` | `CouponDto | null` |

## Used By Endpoints

- [POST ValidateCoupon](../coupons/post-validate-coupon-api-v1-coupons-validate.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

