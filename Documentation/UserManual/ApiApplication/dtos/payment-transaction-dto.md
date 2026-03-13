# PaymentTransactionDto

Data transfer object for PaymentTransactionDto.

## Source

`DR_Admin/DTOs/PaymentTransactionDto.cs`

## TypeScript Interface

```ts
export interface PaymentTransactionDto {
  id: number;
  invoiceId: number;
  paymentMethod: string;
  status: string;
  transactionId: string;
  amount: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `PaymentMethod` | `string` | `string` |
| `Status` | `string` | `string` |
| `TransactionId` | `string` | `string` |
| `Amount` | `decimal` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
