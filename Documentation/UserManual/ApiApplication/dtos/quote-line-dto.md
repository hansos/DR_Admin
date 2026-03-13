# QuoteLineDto

Data transfer object representing a quote line item

## Source

`DR_Admin/DTOs/QuoteLineDto.cs`

## TypeScript Interface

```ts
export interface QuoteLineDto {
  id: number;
  quoteId: number;
  serviceId: number;
  serviceName: string;
  billingCycleId: number;
  billingCycleName: string;
  lineNumber: number;
  description: string;
  quantity: number;
  setupFee: number;
  recurringPrice: number;
  discount: number;
  totalSetupFee: number;
  totalRecurringPrice: number;
  taxRate: number;
  taxAmount: number;
  totalWithTax: number;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `QuoteId` | `int` | `number` |
| `ServiceId` | `int` | `number` |
| `ServiceName` | `string` | `string` |
| `BillingCycleId` | `int` | `number` |
| `BillingCycleName` | `string` | `string` |
| `LineNumber` | `int` | `number` |
| `Description` | `string` | `string` |
| `Quantity` | `int` | `number` |
| `SetupFee` | `decimal` | `number` |
| `RecurringPrice` | `decimal` | `number` |
| `Discount` | `decimal` | `number` |
| `TotalSetupFee` | `decimal` | `number` |
| `TotalRecurringPrice` | `decimal` | `number` |
| `TaxRate` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `TotalWithTax` | `decimal` | `number` |
| `Notes` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
