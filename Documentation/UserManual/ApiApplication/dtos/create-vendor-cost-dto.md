# CreateVendorCostDto

Data transfer object for creating vendor costs

## Source

`DR_Admin/DTOs/CreateVendorCostDto.cs`

## TypeScript Interface

```ts
export interface CreateVendorCostDto {
  invoiceLineId: number;
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
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceLineId` | `int` | `number` |
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
| `Notes` | `string` | `string` |

## Used By Endpoints

- [POST CreateVendorCost](../vendor-costs/post-create-vendor-cost-api-v1-vendor-costs.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

