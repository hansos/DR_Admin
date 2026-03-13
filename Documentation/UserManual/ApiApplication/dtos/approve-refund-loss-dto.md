# ApproveRefundLossDto

Data transfer object for approving refund loss audits

## Source

`DR_Admin/DTOs/ApproveRefundLossDto.cs`

## TypeScript Interface

```ts
export interface ApproveRefundLossDto {
  refundLossAuditId: number;
  approvedByUserId: number;
  isApproved: boolean;
  denialReason: string | null;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RefundLossAuditId` | `int` | `number` |
| `ApprovedByUserId` | `int` | `number` |
| `IsApproved` | `bool` | `boolean` |
| `DenialReason` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
