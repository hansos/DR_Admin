# CreateCouponDto

Data transfer object for creating a coupon

## Source

`DR_Admin/DTOs/CreateCouponDto.cs`

## TypeScript Interface

```ts
export interface CreateCouponDto {
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
  internalNotes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
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
| `InternalNotes` | `string` | `string` |

## Used By Endpoints

- [POST CreateCoupon](../coupons/post-create-coupon-api-v1-coupons.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

