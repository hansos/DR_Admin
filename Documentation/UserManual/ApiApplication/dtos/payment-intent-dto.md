# PaymentIntentDto

Data transfer object representing a payment intent

## Source

`DR_Admin/DTOs/PaymentIntentDto.cs`

## TypeScript Interface

```ts
export interface PaymentIntentDto {
  id: number;
  invoiceId: number | null;
  orderId: number | null;
  customerId: number;
  amount: number;
  currency: string;
  status: PaymentIntentStatus;
  paymentGatewayId: number;
  paymentGatewayProviderCode: string;
  paymentGatewayPublicKey: string;
  gatewayIntentId: string;
  clientSecret: string;
  description: string;
  authorizedAt: string | null;
  capturedAt: string | null;
  failedAt: string | null;
  failureReason: string;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceId` | `int?` | `number | null` |
| `OrderId` | `int?` | `number | null` |
| `CustomerId` | `int` | `number` |
| `Amount` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `Status` | `PaymentIntentStatus` | `PaymentIntentStatus` |
| `PaymentGatewayId` | `int` | `number` |
| `PaymentGatewayProviderCode` | `string` | `string` |
| `PaymentGatewayPublicKey` | `string` | `string` |
| `GatewayIntentId` | `string` | `string` |
| `ClientSecret` | `string` | `string` |
| `Description` | `string` | `string` |
| `AuthorizedAt` | `DateTime?` | `string | null` |
| `CapturedAt` | `DateTime?` | `string | null` |
| `FailedAt` | `DateTime?` | `string | null` |
| `FailureReason` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
