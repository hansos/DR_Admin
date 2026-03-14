# InvoicePaymentDto

Invoice payment DTO

## Source

`DR_Admin/DTOs/PaymentProcessingDtos.cs`

## TypeScript Interface

```ts
export interface InvoicePaymentDto {
  id: number;
  invoiceId: number;
  paymentTransactionId: number;
  amountApplied: number;
  currency: string;
  invoiceBalance: number;
  invoiceTotalAmount: number;
  isFullPayment: boolean;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `PaymentTransactionId` | `int` | `number` |
| `AmountApplied` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `InvoiceBalance` | `decimal` | `number` |
| `InvoiceTotalAmount` | `decimal` | `number` |
| `IsFullPayment` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

