# CreateCreditTransactionDto

Data transfer object for creating a credit transaction

## Source

`DR_Admin/DTOs/CreateCreditTransactionDto.cs`

## TypeScript Interface

```ts
export interface CreateCreditTransactionDto {
  customerId: number;
  type: CreditTransactionType;
  amount: number;
  description: string;
  internalNotes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `Type` | `CreditTransactionType` | `CreditTransactionType` |
| `Amount` | `decimal` | `number` |
| `Description` | `string` | `string` |
| `InternalNotes` | `string` | `string` |

## Used By Endpoints

- [POST CreateCreditTransaction](../customer-credits/post-create-credit-transaction-api-v1-customer-credits-transactions.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

