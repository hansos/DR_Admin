# SubscriptionBillingHistoryDto

Data transfer object representing a subscription billing history record

## Source

`DR_Admin/DTOs/SubscriptionBillingHistoryDto.cs`

## TypeScript Interface

```ts
export interface SubscriptionBillingHistoryDto {
  id: number;
  subscriptionId: number;
  invoiceId: number | null;
  paymentTransactionId: number | null;
  billingDate: string;
  amountCharged: number;
  currencyCode: string;
  status: PaymentTransactionStatus;
  attemptCount: number;
  errorMessage: string;
  periodStart: string;
  periodEnd: string;
  isAutomatic: boolean;
  processedByUserId: number | null;
  notes: string;
  metadata: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `SubscriptionId` | `int` | `number` |
| `InvoiceId` | `int?` | `number | null` |
| `PaymentTransactionId` | `int?` | `number | null` |
| `BillingDate` | `DateTime` | `string` |
| `AmountCharged` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `Status` | `PaymentTransactionStatus` | `PaymentTransactionStatus` |
| `AttemptCount` | `int` | `number` |
| `ErrorMessage` | `string` | `string` |
| `PeriodStart` | `DateTime` | `string` |
| `PeriodEnd` | `DateTime` | `string` |
| `IsAutomatic` | `bool` | `boolean` |
| `ProcessedByUserId` | `int?` | `number | null` |
| `Notes` | `string` | `string` |
| `Metadata` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
