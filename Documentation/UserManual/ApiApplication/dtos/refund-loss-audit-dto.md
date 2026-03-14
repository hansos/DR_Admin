# RefundLossAuditDto

Data transfer object representing financial losses from refunds

## Source

`DR_Admin/DTOs/RefundLossAuditDto.cs`

## TypeScript Interface

```ts
export interface RefundLossAuditDto {
  id: number;
  refundId: number;
  invoiceId: number;
  originalInvoiceAmount: number;
  refundedAmount: number;
  vendorCostUnrecoverable: number;
  netLoss: number;
  currency: string;
  reason: string;
  approvalStatus: ApprovalStatus;
  approvedByUserId: number | null;
  approvedAt: string | null;
  denialReason: string | null;
  internalNotes: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `RefundId` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `OriginalInvoiceAmount` | `decimal` | `number` |
| `RefundedAmount` | `decimal` | `number` |
| `VendorCostUnrecoverable` | `decimal` | `number` |
| `NetLoss` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `Reason` | `string` | `string` |
| `ApprovalStatus` | `ApprovalStatus` | `ApprovalStatus` |
| `ApprovedByUserId` | `int?` | `number | null` |
| `ApprovedAt` | `DateTime?` | `string | null` |
| `DenialReason` | `string?` | `string | null` |
| `InternalNotes` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetRefundLossAuditById](../refund-loss-audits/get-get-refund-loss-audit-by-id-api-v1-refund-loss-audits-id.md)
- [GET GetRefundLossAuditByRefundId](../refund-loss-audits/get-get-refund-loss-audit-by-refund-id-api-v1-refund-loss-audits-refund-refundid.md)
- [POST ApproveRefundLoss](../refund-loss-audits/post-approve-refund-loss-api-v1-refund-loss-audits-approve.md)
- [POST CreateRefundLossAudit](../refund-loss-audits/post-create-refund-loss-audit-api-v1-refund-loss-audits.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

