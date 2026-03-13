# VendorCostSummaryDto

Data transfer object representing vendor cost summary for an invoice

## Source

`DR_Admin/DTOs/VendorCostSummaryDto.cs`

## TypeScript Interface

```ts
export interface VendorCostSummaryDto {
  invoiceId: number;
  invoiceNumber: string;
  invoiceTotal: number;
  currencyCode: string;
  totalVendorCosts: number;
  totalPaidVendorCosts: number;
  totalUnpaidVendorCosts: number;
  totalRefundableVendorCosts: number;
  totalNonRefundableVendorCosts: number;
  grossProfit: number;
  grossProfitMargin: number;
  vendorCosts: VendorCostDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceId` | `int` | `number` |
| `InvoiceNumber` | `string` | `string` |
| `InvoiceTotal` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `TotalVendorCosts` | `decimal` | `number` |
| `TotalPaidVendorCosts` | `decimal` | `number` |
| `TotalUnpaidVendorCosts` | `decimal` | `number` |
| `TotalRefundableVendorCosts` | `decimal` | `number` |
| `TotalNonRefundableVendorCosts` | `decimal` | `number` |
| `GrossProfit` | `decimal` | `number` |
| `GrossProfitMargin` | `decimal` | `number` |
| `VendorCosts` | `List<VendorCostDto>` | `VendorCostDto[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
