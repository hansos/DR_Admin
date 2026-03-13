# CreateRefundDto

Data transfer object for creating a refund

## Source

`DR_Admin/DTOs/CreateRefundDto.cs`

## TypeScript Interface

```ts
export interface CreateRefundDto {
  paymentTransactionId: number;
  amount: number | null;
  reason: string;
  internalNotes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `PaymentTransactionId` | `int` | `number` |
| `Amount` | `decimal?` | `number | null` |
| `Reason` | `string` | `string` |
| `InternalNotes` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
