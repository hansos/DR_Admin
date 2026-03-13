# CreateRefundLossAuditDto

Data transfer object for creating refund loss audits

## Source

`DR_Admin/DTOs/CreateRefundLossAuditDto.cs`

## TypeScript Interface

```ts
export interface CreateRefundLossAuditDto {
  refundId: number;
  invoiceId: number;
  originalInvoiceAmount: number;
  refundedAmount: number;
  vendorCostUnrecoverable: number;
  netLoss: number;
  currency: string;
  reason: string;
  internalNotes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RefundId` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `OriginalInvoiceAmount` | `decimal` | `number` |
| `RefundedAmount` | `decimal` | `number` |
| `VendorCostUnrecoverable` | `decimal` | `number` |
| `NetLoss` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `Reason` | `string` | `string` |
| `InternalNotes` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
