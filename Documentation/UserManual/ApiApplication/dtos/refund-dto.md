# RefundDto

Data transfer object representing a refund

## Source

`DR_Admin/DTOs/RefundDto.cs`

## TypeScript Interface

```ts
export interface RefundDto {
  id: number;
  paymentTransactionId: number;
  invoiceId: number;
  invoiceNumber: string;
  amount: number;
  reason: string;
  status: RefundStatus;
  refundTransactionId: string;
  processedAt: string | null;
  failedAt: string | null;
  failureReason: string;
  initiatedByUserId: number | null;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `PaymentTransactionId` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `InvoiceNumber` | `string` | `string` |
| `Amount` | `decimal` | `number` |
| `Reason` | `string` | `string` |
| `Status` | `RefundStatus` | `RefundStatus` |
| `RefundTransactionId` | `string` | `string` |
| `ProcessedAt` | `DateTime?` | `string | null` |
| `FailedAt` | `DateTime?` | `string | null` |
| `FailureReason` | `string` | `string` |
| `InitiatedByUserId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetRefundById](../refunds/get-get-refund-by-id-api-v1-refunds-id.md)
- [POST CreateRefund](../refunds/post-create-refund-api-v1-refunds.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

