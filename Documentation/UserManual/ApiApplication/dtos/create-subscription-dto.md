# CreateSubscriptionDto

Data transfer object for creating a new subscription

## Source

`DR_Admin/DTOs/SubscriptionDto.cs`

## TypeScript Interface

```ts
export interface CreateSubscriptionDto {
  customerId: number;
  serviceId: number | null;
  billingCycleId: number;
  customerPaymentMethodId: number | null;
  paymentGatewayId: number | null;
  startDate: string | null;
  endDate: string | null;
  amount: number;
  currencyCode: string;
  billingPeriodCount: number;
  billingPeriodUnit: SubscriptionPeriodUnit;
  trialDays: number;
  maxRetryAttempts: number;
  metadata: string;
  notes: string;
  quantity: number;
  sendEmailNotifications: boolean;
  autoRetryFailedPayments: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `BillingCycleId` | `int` | `number` |
| `CustomerPaymentMethodId` | `int?` | `number | null` |
| `PaymentGatewayId` | `int?` | `number | null` |
| `StartDate` | `DateTime?` | `string | null` |
| `EndDate` | `DateTime?` | `string | null` |
| `Amount` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `BillingPeriodCount` | `int` | `number` |
| `BillingPeriodUnit` | `SubscriptionPeriodUnit` | `SubscriptionPeriodUnit` |
| `TrialDays` | `int` | `number` |
| `MaxRetryAttempts` | `int` | `number` |
| `Metadata` | `string` | `string` |
| `Notes` | `string` | `string` |
| `Quantity` | `int` | `number` |
| `SendEmailNotifications` | `bool` | `boolean` |
| `AutoRetryFailedPayments` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
