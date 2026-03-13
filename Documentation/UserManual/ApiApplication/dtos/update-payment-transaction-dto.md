# UpdatePaymentTransactionDto

Data transfer object for UpdatePaymentTransactionDto.

## Source

`DR_Admin/DTOs/PaymentTransactionDto.cs`

## TypeScript Interface

```ts
export interface UpdatePaymentTransactionDto {
  invoiceId: number;
  paymentMethod: string;
  status: string;
  transactionId: string;
  amount: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceId` | `int` | `number` |
| `PaymentMethod` | `string` | `string` |
| `Status` | `string` | `string` |
| `TransactionId` | `string` | `string` |
| `Amount` | `decimal` | `number` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
