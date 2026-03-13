# CreatePaymentIntentDto

Data transfer object for creating a payment intent

## Source

`DR_Admin/DTOs/CreatePaymentIntentDto.cs`

## TypeScript Interface

```ts
export interface CreatePaymentIntentDto {
  invoiceId: number | null;
  orderId: number | null;
  amount: number;
  currency: string;
  paymentGatewayId: number;
  paymentInstrument: string;
  returnUrl: string;
  cancelUrl: string;
  description: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceId` | `int?` | `number | null` |
| `OrderId` | `int?` | `number | null` |
| `Amount` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `PaymentGatewayId` | `int` | `number` |
| `PaymentInstrument` | `string` | `string` |
| `ReturnUrl` | `string` | `string` |
| `CancelUrl` | `string` | `string` |
| `Description` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
