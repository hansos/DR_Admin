# ValidateCouponDto

Data transfer object for validating a coupon

## Source

`DR_Admin/DTOs/ValidateCouponDto.cs`

## TypeScript Interface

```ts
export interface ValidateCouponDto {
  code: string;
  customerId: number;
  totalAmount: number;
  serviceTypeIds: number[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `CustomerId` | `int` | `number` |
| `TotalAmount` | `decimal` | `number` |
| `ServiceTypeIds` | `List<int>` | `number[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
