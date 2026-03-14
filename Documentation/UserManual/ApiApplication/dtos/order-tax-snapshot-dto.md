# OrderTaxSnapshotDto

Data transfer object representing an immutable order tax snapshot.

## Source

`DR_Admin/DTOs/OrderTaxSnapshotDto.cs`

## TypeScript Interface

```ts
export interface OrderTaxSnapshotDto {
  id: number;
  orderId: number;
  taxJurisdictionId: number | null;
  buyerCountryCode: string;
  buyerStateCode: string | null;
  buyerType: CustomerType;
  buyerTaxId: string;
  buyerTaxIdValidated: boolean;
  taxCurrencyCode: string;
  displayCurrencyCode: string;
  exchangeRate: number | null;
  exchangeRateDate: string | null;
  exchangeRateSource: CurrencyRateSource | null;
  taxDeterminationEvidenceId: number | null;
  netAmount: number;
  taxAmount: number;
  grossAmount: number;
  appliedTaxRate: number;
  appliedTaxName: string;
  reverseChargeApplied: boolean;
  ruleVersion: string;
  idempotencyKey: string | null;
  calculationInputsJson: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `OrderId` | `int` | `number` |
| `TaxJurisdictionId` | `int?` | `number | null` |
| `BuyerCountryCode` | `string` | `string` |
| `BuyerStateCode` | `string?` | `string | null` |
| `BuyerType` | `CustomerType` | `CustomerType` |
| `BuyerTaxId` | `string` | `string` |
| `BuyerTaxIdValidated` | `bool` | `boolean` |
| `TaxCurrencyCode` | `string` | `string` |
| `DisplayCurrencyCode` | `string` | `string` |
| `ExchangeRate` | `decimal?` | `number | null` |
| `ExchangeRateDate` | `DateTime?` | `string | null` |
| `ExchangeRateSource` | `CurrencyRateSource?` | `CurrencyRateSource | null` |
| `TaxDeterminationEvidenceId` | `int?` | `number | null` |
| `NetAmount` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `GrossAmount` | `decimal` | `number` |
| `AppliedTaxRate` | `decimal` | `number` |
| `AppliedTaxName` | `string` | `string` |
| `ReverseChargeApplied` | `bool` | `boolean` |
| `RuleVersion` | `string` | `string` |
| `IdempotencyKey` | `string?` | `string | null` |
| `CalculationInputsJson` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetOrderTaxSnapshotById](../order-tax-snapshots/get-get-order-tax-snapshot-by-id-api-v1-order-tax-snapshots-id.md)
- [POST CreateOrderTaxSnapshot](../order-tax-snapshots/post-create-order-tax-snapshot-api-v1-order-tax-snapshots.md)
- [PUT UpdateOrderTaxSnapshot](../order-tax-snapshots/put-update-order-tax-snapshot-api-v1-order-tax-snapshots-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

