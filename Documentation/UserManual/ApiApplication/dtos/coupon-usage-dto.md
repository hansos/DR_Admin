# CouponUsageDto

Data transfer object representing a coupon usage entry

## Source

`DR_Admin/DTOs/CouponUsageDto.cs`

## TypeScript Interface

```ts
export interface CouponUsageDto {
  id: number;
  couponId: number;
  couponCode: string;
  couponName: string;
  customerId: number;
  customerName: string;
  quoteId: number | null;
  orderId: number | null;
  discountAmount: number;
  usedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CouponId` | `int` | `number` |
| `CouponCode` | `string` | `string` |
| `CouponName` | `string` | `string` |
| `CustomerId` | `int` | `number` |
| `CustomerName` | `string` | `string` |
| `QuoteId` | `int?` | `number | null` |
| `OrderId` | `int?` | `number | null` |
| `DiscountAmount` | `decimal` | `number` |
| `UsedAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

