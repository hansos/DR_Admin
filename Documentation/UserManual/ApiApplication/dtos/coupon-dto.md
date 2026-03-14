# CouponDto

Data transfer object representing a coupon

## Source

`DR_Admin/DTOs/CouponDto.cs`

## TypeScript Interface

```ts
export interface CouponDto {
  id: number;
  code: string;
  name: string;
  description: string;
  type: CouponType;
  value: number;
  appliesTo: DiscountAppliesTo;
  recurrenceType: CouponRecurrenceType;
  recurringYears: number | null;
  minimumAmount: number | null;
  maximumDiscount: number | null;
  validFrom: string;
  validUntil: string;
  maxUsages: number | null;
  maxUsagesPerCustomer: number | null;
  isActive: boolean;
  allowedServiceTypeIds: number[] | null;
  usageCount: number;
  internalNotes: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `Type` | `CouponType` | `CouponType` |
| `Value` | `decimal` | `number` |
| `AppliesTo` | `DiscountAppliesTo` | `DiscountAppliesTo` |
| `RecurrenceType` | `CouponRecurrenceType` | `CouponRecurrenceType` |
| `RecurringYears` | `int?` | `number | null` |
| `MinimumAmount` | `decimal?` | `number | null` |
| `MaximumDiscount` | `decimal?` | `number | null` |
| `ValidFrom` | `DateTime` | `string` |
| `ValidUntil` | `DateTime` | `string` |
| `MaxUsages` | `int?` | `number | null` |
| `MaxUsagesPerCustomer` | `int?` | `number | null` |
| `IsActive` | `bool` | `boolean` |
| `AllowedServiceTypeIds` | `List<int>?` | `number[] | null` |
| `UsageCount` | `int` | `number` |
| `InternalNotes` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetCouponByCode](../coupons/get-get-coupon-by-code-api-v1-coupons-code-code.md)
- [GET GetCouponById](../coupons/get-get-coupon-by-id-api-v1-coupons-id.md)
- [POST CreateCoupon](../coupons/post-create-coupon-api-v1-coupons.md)
- [PUT UpdateCoupon](../coupons/put-update-coupon-api-v1-coupons-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

