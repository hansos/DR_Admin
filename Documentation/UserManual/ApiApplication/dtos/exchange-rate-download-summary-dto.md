# ExchangeRateDownloadSummaryDto

DTO for summary statistics of exchange rate downloads

## Source

`DR_Admin/DTOs/ExchangeRateDownloadSummaryDto.cs`

## TypeScript Interface

```ts
export interface ExchangeRateDownloadSummaryDto {
  totalDownloads: number;
  successfulDownloads: number;
  failedDownloads: number;
  totalRatesDownloaded: number;
  totalRatesAdded: number;
  totalRatesUpdated: number;
  lastSuccessfulDownload: string | null;
  lastFailedDownload: string | null;
  averageDurationMs: number;
  successRate: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TotalDownloads` | `int` | `number` |
| `SuccessfulDownloads` | `int` | `number` |
| `FailedDownloads` | `int` | `number` |
| `TotalRatesDownloaded` | `int` | `number` |
| `TotalRatesAdded` | `int` | `number` |
| `TotalRatesUpdated` | `int` | `number` |
| `LastSuccessfulDownload` | `DateTime?` | `string | null` |
| `LastFailedDownload` | `DateTime?` | `string | null` |
| `AverageDurationMs` | `long` | `number` |
| `SuccessRate` | `double` | `number` |

## Used By Endpoints

- [GET GetSummary](../exchange-rate-download-logs/get-get-summary-api-exchange-rate-download-logs-summary.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

