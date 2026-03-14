# VendorCostDto

Data transfer object representing vendor costs for invoice line items

## Source

`DR_Admin/DTOs/VendorCostDto.cs`

## TypeScript Interface

```ts
export interface VendorCostDto {
  id: number;
  invoiceLineId: number;
  vendorPayoutId: number | null;
  vendorType: VendorType;
  vendorId: number | null;
  vendorName: string;
  vendorCurrency: string;
  vendorAmount: number;
  baseCurrency: string;
  baseAmount: number;
  exchangeRate: number;
  exchangeRateDate: string;
  isRefundable: boolean;
  refundPolicy: RefundPolicy;
  refundDeadline: string | null;
  status: VendorCostStatus;
  notes: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceLineId` | `int` | `number` |
| `VendorPayoutId` | `int?` | `number | null` |
| `VendorType` | `VendorType` | `VendorType` |
| `VendorId` | `int?` | `number | null` |
| `VendorName` | `string` | `string` |
| `VendorCurrency` | `string` | `string` |
| `VendorAmount` | `decimal` | `number` |
| `BaseCurrency` | `string` | `string` |
| `BaseAmount` | `decimal` | `number` |
| `ExchangeRate` | `decimal` | `number` |
| `ExchangeRateDate` | `DateTime` | `string` |
| `IsRefundable` | `bool` | `boolean` |
| `RefundPolicy` | `RefundPolicy` | `RefundPolicy` |
| `RefundDeadline` | `DateTime?` | `string | null` |
| `Status` | `VendorCostStatus` | `VendorCostStatus` |
| `Notes` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetVendorCostById](../vendor-costs/get-get-vendor-cost-by-id-api-v1-vendor-costs-id.md)
- [POST CreateVendorCost](../vendor-costs/post-create-vendor-cost-api-v1-vendor-costs.md)
- [PUT UpdateVendorCost](../vendor-costs/put-update-vendor-cost-api-v1-vendor-costs-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

