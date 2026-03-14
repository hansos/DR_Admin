# CreateQuoteLineDto

Data transfer object for creating a quote line item

## Source

`DR_Admin/DTOs/CreateQuoteLineDto.cs`

## TypeScript Interface

```ts
export interface CreateQuoteLineDto {
  serviceId: number;
  billingCycleId: number;
  quantity: number;
  description: string | null;
  setupFee: number | null;
  recurringPrice: number | null;
  discount: number | null;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ServiceId` | `int` | `number` |
| `BillingCycleId` | `int` | `number` |
| `Quantity` | `int` | `number` |
| `Description` | `string?` | `string | null` |
| `SetupFee` | `decimal?` | `number | null` |
| `RecurringPrice` | `decimal?` | `number | null` |
| `Discount` | `decimal?` | `number | null` |
| `Notes` | `string` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

