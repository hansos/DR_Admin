# VendorPayoutDto

Data transfer object representing an outbound payment to a vendor

## Source

`DR_Admin/DTOs/VendorPayoutDto.cs`

## TypeScript Interface

```ts
export interface VendorPayoutDto {
  id: number;
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
  status: VendorPayoutStatus;
  scheduledDate: string;
  processedDate: string | null;
  failureReason: string;
  failureCount: number;
  transactionReference: string;
  requiresManualIntervention: boolean;
  interventionReason: string;
  interventionResolvedAt: string | null;
  interventionResolvedByUserId: number | null;
  internalNotes: string;
  vendorCosts: VendorCostDto[];
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property                       | C# Type               | TypeScript Type      |
| ------------------------------ | --------------------- | -------------------- |
| `Id`                           | `int`                 | `number`             |
| `VendorId`                     | `int`                 | `number`             |
| `VendorType`                   | `VendorType`          | `VendorType`         |
| `VendorName`                   | `string`              | `string`             |
| `PayoutMethod`                 | `PayoutMethod`        | `PayoutMethod`       |
| `VendorCurrency`               | `string`              | `string`             |
| `VendorAmount`                 | `decimal`             | `number`             |
| `BaseCurrency`                 | `string`              | `string`             |
| `BaseAmount`                   | `decimal`             | `number`             |
| `ExchangeRate`                 | `decimal`             | `number`             |
| `ExchangeRateDate`             | `DateTime`            | `string`             |
| `Status`                       | `VendorPayoutStatus`  | `VendorPayoutStatus` |
| `ScheduledDate`                | `DateTime`            | `string`             |
| `ProcessedDate`                | `DateTime?`           | `string              |
| `FailureReason`                | `string`              | `string`             |
| `FailureCount`                 | `int`                 | `number`             |
| `TransactionReference`         | `string`              | `string`             |
| `RequiresManualIntervention`   | `bool`                | `boolean`            |
| `InterventionReason`           | `string`              | `string`             |
| `InterventionResolvedAt`       | `DateTime?`           | `string              |
| `InterventionResolvedByUserId` | `int?`                | `number              |
| `InternalNotes`                | `string`              | `string`             |
| `VendorCosts`                  | `List<VendorCostDto>` | `VendorCostDto[]`    |
| `CreatedAt`                    | `DateTime`            | `string`             |
| `UpdatedAt`                    | `DateTime`            | `string`             |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
