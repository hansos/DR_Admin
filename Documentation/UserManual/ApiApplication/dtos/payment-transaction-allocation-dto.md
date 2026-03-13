# PaymentTransactionAllocationDto

Represents an invoice allocation row linked to a payment transaction.

## Source

`DR_Admin/DTOs/PaymentTransactionListDto.cs`

## TypeScript Interface

```ts
export interface PaymentTransactionAllocationDto {
  id: number;
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
| `AmountApplied` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `InvoiceBalance` | `decimal` | `number` |
| `InvoiceTotalAmount` | `decimal` | `number` |
| `IsFullPayment` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
