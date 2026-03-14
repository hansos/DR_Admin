# UpdateSubscriptionDto

Data transfer object for updating an existing subscription

## Source

`DR_Admin/DTOs/SubscriptionDto.cs`

## TypeScript Interface

```ts
export interface UpdateSubscriptionDto {
  customerPaymentMethodId: number | null;
  paymentGatewayId: number | null;
  endDate: string | null;
  amount: number | null;
  currencyCode: string | null;
  billingPeriodCount: number | null;
  billingPeriodUnit: SubscriptionPeriodUnit | null;
  maxRetryAttempts: number | null;
  metadata: string | null;
  notes: string | null;
  quantity: number | null;
  sendEmailNotifications: boolean | null;
  autoRetryFailedPayments: boolean | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerPaymentMethodId` | `int?` | `number | null` |
| `PaymentGatewayId` | `int?` | `number | null` |
| `EndDate` | `DateTime?` | `string | null` |
| `Amount` | `decimal?` | `number | null` |
| `CurrencyCode` | `string?` | `string | null` |
| `BillingPeriodCount` | `int?` | `number | null` |
| `BillingPeriodUnit` | `SubscriptionPeriodUnit?` | `SubscriptionPeriodUnit | null` |
| `MaxRetryAttempts` | `int?` | `number | null` |
| `Metadata` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |
| `Quantity` | `int?` | `number | null` |
| `SendEmailNotifications` | `bool?` | `boolean | null` |
| `AutoRetryFailedPayments` | `bool?` | `boolean | null` |

## Used By Endpoints

- [PUT UpdateSubscription](../subscriptions/put-update-subscription-api-v1-subscriptions-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

