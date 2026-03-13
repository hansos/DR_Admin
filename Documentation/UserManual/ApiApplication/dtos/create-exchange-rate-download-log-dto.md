# CreateExchangeRateDownloadLogDto

DTO for creating an exchange rate download log entry

## Source

`DR_Admin/DTOs/CreateExchangeRateDownloadLogDto.cs`

## TypeScript Interface

```ts
export interface CreateExchangeRateDownloadLogDto {
  baseCurrency: string;
  targetCurrency: string | null;
  source: CurrencyRateSource;
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
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `BaseCurrency` | `string` | `string` |
| `TargetCurrency` | `string?` | `string | null` |
| `Source` | `CurrencyRateSource` | `CurrencyRateSource` |
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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
