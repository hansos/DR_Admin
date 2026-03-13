# CreateVendorPayoutDto

Data transfer object for creating vendor payouts

## Source

`DR_Admin/DTOs/CreateVendorPayoutDto.cs`

## TypeScript Interface

```ts
export interface CreateVendorPayoutDto {
  vendorId: number;
  vendorType: VendorType;
  vendorName: string;
  payoutMethod: PayoutMethod;
  vendorCurrency: string;
  vendorAmount: number;
  baseCurrency: string;
  baseAmount: number;
  exchangeRate: number;
  exchangeRateDate: string;
  scheduledDate: string;
  internalNotes: string;
  vendorCostIds: number[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `VendorId` | `int` | `number` |
| `VendorType` | `VendorType` | `VendorType` |
| `VendorName` | `string` | `string` |
| `PayoutMethod` | `PayoutMethod` | `PayoutMethod` |
| `VendorCurrency` | `string` | `string` |
| `VendorAmount` | `decimal` | `number` |
| `BaseCurrency` | `string` | `string` |
| `BaseAmount` | `decimal` | `number` |
| `ExchangeRate` | `decimal` | `number` |
| `ExchangeRateDate` | `DateTime` | `string` |
| `ScheduledDate` | `DateTime` | `string` |
| `InternalNotes` | `string` | `string` |
| `VendorCostIds` | `List<int>` | `number[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
