# SubscriptionDto

Data transfer object representing a subscription

## Source

`DR_Admin/DTOs/SubscriptionDto.cs`

## TypeScript Interface

```ts
export interface SubscriptionDto {
  id: number;
  customerId: number;
  serviceId: number | null;
  billingCycleId: number;
  customerPaymentMethodId: number | null;
  paymentGatewayId: number | null;
  status: SubscriptionStatus;
  startDate: string;
  endDate: string | null;
  nextBillingDate: string;
  currentPeriodStart: string;
  currentPeriodEnd: string;
  amount: number;
  currencyCode: string;
  billingPeriodCount: number;
  billingPeriodUnit: SubscriptionPeriodUnit;
  trialEndDate: string | null;
  isInTrial: boolean;
  retryCount: number;
  maxRetryAttempts: number;
  lastBillingAttempt: string | null;
  lastSuccessfulBilling: string | null;
  cancelledAt: string | null;
  cancellationReason: string;
  pausedAt: string | null;
  pauseReason: string;
  metadata: string;
  notes: string;
  quantity: number;
  sendEmailNotifications: boolean;
  autoRetryFailedPayments: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `BillingCycleId` | `int` | `number` |
| `CustomerPaymentMethodId` | `int?` | `number | null` |
| `PaymentGatewayId` | `int?` | `number | null` |
| `Status` | `SubscriptionStatus` | `SubscriptionStatus` |
| `StartDate` | `DateTime` | `string` |
| `EndDate` | `DateTime?` | `string | null` |
| `NextBillingDate` | `DateTime` | `string` |
| `CurrentPeriodStart` | `DateTime` | `string` |
| `CurrentPeriodEnd` | `DateTime` | `string` |
| `Amount` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `BillingPeriodCount` | `int` | `number` |
| `BillingPeriodUnit` | `SubscriptionPeriodUnit` | `SubscriptionPeriodUnit` |
| `TrialEndDate` | `DateTime?` | `string | null` |
| `IsInTrial` | `bool` | `boolean` |
| `RetryCount` | `int` | `number` |
| `MaxRetryAttempts` | `int` | `number` |
| `LastBillingAttempt` | `DateTime?` | `string | null` |
| `LastSuccessfulBilling` | `DateTime?` | `string | null` |
| `CancelledAt` | `DateTime?` | `string | null` |
| `CancellationReason` | `string` | `string` |
| `PausedAt` | `DateTime?` | `string | null` |
| `PauseReason` | `string` | `string` |
| `Metadata` | `string` | `string` |
| `Notes` | `string` | `string` |
| `Quantity` | `int` | `number` |
| `SendEmailNotifications` | `bool` | `boolean` |
| `AutoRetryFailedPayments` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetSubscriptionById](../subscriptions/get-get-subscription-by-id-api-v1-subscriptions-id.md)
- [POST CancelSubscription](../subscriptions/post-cancel-subscription-api-v1-subscriptions-id-cancel.md)
- [POST CreateSubscription](../subscriptions/post-create-subscription-api-v1-subscriptions.md)
- [POST PauseSubscription](../subscriptions/post-pause-subscription-api-v1-subscriptions-id-pause.md)
- [POST ResumeSubscription](../subscriptions/post-resume-subscription-api-v1-subscriptions-id-resume.md)
- [PUT UpdateSubscription](../subscriptions/put-update-subscription-api-v1-subscriptions-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

