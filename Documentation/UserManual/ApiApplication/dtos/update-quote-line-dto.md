# UpdateQuoteLineDto

Data transfer object for updating a quote line item

## Source

`DR_Admin/DTOs/UpdateQuoteLineDto.cs`

## TypeScript Interface

```ts
export interface UpdateQuoteLineDto {
  id: number;
  serviceId: number;
  billingCycleId: number;
  quantity: number;
  description: string;
  setupFee: number;
  recurringPrice: number;
  discount: number;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ServiceId` | `int` | `number` |
| `BillingCycleId` | `int` | `number` |
| `Quantity` | `int` | `number` |
| `Description` | `string` | `string` |
| `SetupFee` | `decimal` | `number` |
| `RecurringPrice` | `decimal` | `number` |
| `Discount` | `decimal` | `number` |
| `Notes` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
