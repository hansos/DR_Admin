# UpdateOrderTaxSnapshotDto

Data transfer object for updating an order tax snapshot.

## Source

`DR_Admin/DTOs/UpdateOrderTaxSnapshotDto.cs`

## TypeScript Interface

```ts
export interface UpdateOrderTaxSnapshotDto {
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
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
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

## Used By Endpoints

- [PUT UpdateOrderTaxSnapshot](../order-tax-snapshots/put-update-order-tax-snapshot-api-v1-order-tax-snapshots-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

