# CreateOrderDto

Data transfer object for creating a new order

## Source

`DR_Admin/DTOs/OrderDto.cs`

## TypeScript Interface

```ts
export interface CreateOrderDto {
  customerId: number;
  serviceId: number | null;
  quoteId: number | null;
  orderType: OrderType;
  startDate: string;
  endDate: string;
  nextBillingDate: string;
  setupFee: number | null;
  recurringAmount: number | null;
  couponCode: string | null;
  autoRenew: boolean;
  orderLines: CreateOrderLineDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `QuoteId` | `int?` | `number | null` |
| `OrderType` | `OrderType` | `OrderType` |
| `StartDate` | `DateTime` | `string` |
| `EndDate` | `DateTime` | `string` |
| `NextBillingDate` | `DateTime` | `string` |
| `SetupFee` | `decimal?` | `number | null` |
| `RecurringAmount` | `decimal?` | `number | null` |
| `CouponCode` | `string?` | `string | null` |
| `AutoRenew` | `bool` | `boolean` |
| `OrderLines` | `List<CreateOrderLineDto>` | `CreateOrderLineDto[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
