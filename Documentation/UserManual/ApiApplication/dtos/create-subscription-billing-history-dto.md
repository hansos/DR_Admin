# CreateSubscriptionBillingHistoryDto

Data transfer object for creating a new subscription billing history record

## Source

`DR_Admin/DTOs/SubscriptionBillingHistoryDto.cs`

## TypeScript Interface

```ts
export interface CreateSubscriptionBillingHistoryDto {
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
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
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

## Used By Endpoints

- [POST CreateBillingHistory](../subscription-billing-histories/post-create-billing-history-api-v1-subscription-billing-histories.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

