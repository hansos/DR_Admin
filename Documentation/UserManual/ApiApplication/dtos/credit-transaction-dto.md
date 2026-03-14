# CreditTransactionDto

Data transfer object representing a credit transaction

## Source

`DR_Admin/DTOs/CreditTransactionDto.cs`

## TypeScript Interface

```ts
export interface CreditTransactionDto {
  id: number;
  customerCreditId: number;
  type: CreditTransactionType;
  amount: number;
  invoiceId: number | null;
  paymentTransactionId: number | null;
  description: string;
  balanceAfter: number;
  createdByUserId: number | null;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerCreditId` | `int` | `number` |
| `Type` | `CreditTransactionType` | `CreditTransactionType` |
| `Amount` | `decimal` | `number` |
| `InvoiceId` | `int?` | `number | null` |
| `PaymentTransactionId` | `int?` | `number | null` |
| `Description` | `string` | `string` |
| `BalanceAfter` | `decimal` | `number` |
| `CreatedByUserId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [POST CreateCreditTransaction](../customer-credits/post-create-credit-transaction-api-v1-customer-credits-transactions.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

