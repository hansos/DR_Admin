# TaxQuoteResultDto

Represents tax calculation quote or finalization output.

## Source

`DR_Admin/DTOs/TaxQuoteResultDto.cs`

## TypeScript Interface

```ts
export interface TaxQuoteResultDto {
  snapshotId: number | null;
  orderId: number | null;
  taxJurisdictionId: number | null;
  taxName: string;
  taxRate: number;
  reverseChargeApplied: boolean;
  ruleVersion: string;
  taxCurrencyCode: string;
  displayCurrencyCode: string;
  exchangeRate: number | null;
  exchangeRateDate: string | null;
  exchangeRateSource: CurrencyRateSource | null;
  netAmount: number;
  taxAmount: number;
  grossAmount: number;
  buyerTaxIdValidated: boolean;
  legalNote: string;
  taxDeterminationEvidenceId: number | null;
  lines: TaxQuoteLineResultDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `SnapshotId` | `int?` | `number | null` |
| `OrderId` | `int?` | `number | null` |
| `TaxJurisdictionId` | `int?` | `number | null` |
| `TaxName` | `string` | `string` |
| `TaxRate` | `decimal` | `number` |
| `ReverseChargeApplied` | `bool` | `boolean` |
| `RuleVersion` | `string` | `string` |
| `TaxCurrencyCode` | `string` | `string` |
| `DisplayCurrencyCode` | `string` | `string` |
| `ExchangeRate` | `decimal?` | `number | null` |
| `ExchangeRateDate` | `DateTime?` | `string | null` |
| `ExchangeRateSource` | `CurrencyRateSource?` | `CurrencyRateSource | null` |
| `NetAmount` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `GrossAmount` | `decimal` | `number` |
| `BuyerTaxIdValidated` | `bool` | `boolean` |
| `LegalNote` | `string` | `string` |
| `TaxDeterminationEvidenceId` | `int?` | `number | null` |
| `Lines` | `ICollection<TaxQuoteLineResultDto>` | `TaxQuoteLineResultDto[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
