# ExchangeRateDownloadLogDto

DTO for exchange rate download log

## Source

`DR_Admin/DTOs/ExchangeRateDownloadLogDto.cs`

## TypeScript Interface

```ts
export interface ExchangeRateDownloadLogDto {
  id: number;
  baseCurrency: string;
  targetCurrency: string | null;
  source: CurrencyRateSource;
  sourceName: string;
  success: boolean;
  downloadTimestamp: string;
  ratesDownloaded: number;
  ratesAdded: number;
  ratesUpdated: number;
  errorMessage: string | null;
  errorCode: string | null;
  durationMs: number;
  notes: string | null;
  isStartupDownload: boolean;
  isScheduledDownload: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `BaseCurrency` | `string` | `string` |
| `TargetCurrency` | `string?` | `string | null` |
| `Source` | `CurrencyRateSource` | `CurrencyRateSource` |
| `SourceName` | `string` | `string` |
| `Success` | `bool` | `boolean` |
| `DownloadTimestamp` | `DateTime` | `string` |
| `RatesDownloaded` | `int` | `number` |
| `RatesAdded` | `int` | `number` |
| `RatesUpdated` | `int` | `number` |
| `ErrorMessage` | `string?` | `string | null` |
| `ErrorCode` | `string?` | `string | null` |
| `DurationMs` | `long` | `number` |
| `Notes` | `string?` | `string | null` |
| `IsStartupDownload` | `bool` | `boolean` |
| `IsScheduledDownload` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
