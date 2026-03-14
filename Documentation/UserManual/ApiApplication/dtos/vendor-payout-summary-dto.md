# VendorPayoutSummaryDto

Data transfer object representing vendor payout summary

## Source

`DR_Admin/DTOs/VendorPayoutSummaryDto.cs`

## TypeScript Interface

```ts
export interface VendorPayoutSummaryDto {
  vendorId: number;
  vendorName: string;
  vendorType: VendorType;
  totalPending: number;
  totalProcessing: number;
  totalPaid: number;
  totalFailed: number;
  currencyCode: string;
  pendingCount: number;
  requiresInterventionCount: number;
  nextScheduledDate: string | null;
  recentPayouts: VendorPayoutDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `VendorId` | `int` | `number` |
| `VendorName` | `string` | `string` |
| `VendorType` | `VendorType` | `VendorType` |
| `TotalPending` | `decimal` | `number` |
| `TotalProcessing` | `decimal` | `number` |
| `TotalPaid` | `decimal` | `number` |
| `TotalFailed` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `PendingCount` | `int` | `number` |
| `RequiresInterventionCount` | `int` | `number` |
| `NextScheduledDate` | `DateTime?` | `string | null` |
| `RecentPayouts` | `List<VendorPayoutDto>` | `VendorPayoutDto[]` |

## Used By Endpoints

- [GET GetVendorPayoutSummaryByVendorId](../vendor-payouts/get-get-vendor-payout-summary-by-vendor-id-api-v1-vendor-payouts-summary-vendor-vendorid.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

