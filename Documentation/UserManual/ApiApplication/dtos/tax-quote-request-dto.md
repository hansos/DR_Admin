# TaxQuoteRequestDto

Represents tax quote or finalize input payload.

## Source

`DR_Admin/DTOs/TaxQuoteRequestDto.cs`

## TypeScript Interface

```ts
export interface TaxQuoteRequestDto {
  orderId: number | null;
  customerId: number | null;
  buyerCountryCode: string;
  buyerStateCode: string | null;
  buyerType: CustomerType;
  buyerTaxId: string;
  validateBuyerTaxId: boolean;
  transactionDate: string;
  taxCurrencyCode: string;
  displayCurrencyCode: string;
  exchangeRate: number | null;
  exchangeRateDate: string | null;
  requireTrustedExchangeRate: boolean;
  billingCountryCode: string;
  ipAddress: string;
  idempotencyKey: string | null;
  lines: TaxQuoteLineRequestDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `OrderId` | `int?` | `number | null` |
| `CustomerId` | `int?` | `number | null` |
| `BuyerCountryCode` | `string` | `string` |
| `BuyerStateCode` | `string?` | `string | null` |
| `BuyerType` | `CustomerType` | `CustomerType` |
| `BuyerTaxId` | `string` | `string` |
| `ValidateBuyerTaxId` | `bool` | `boolean` |
| `TransactionDate` | `DateTime` | `string` |
| `TaxCurrencyCode` | `string` | `string` |
| `DisplayCurrencyCode` | `string` | `string` |
| `ExchangeRate` | `decimal?` | `number | null` |
| `ExchangeRateDate` | `DateTime?` | `string | null` |
| `RequireTrustedExchangeRate` | `bool` | `boolean` |
| `BillingCountryCode` | `string` | `string` |
| `IpAddress` | `string` | `string` |
| `IdempotencyKey` | `string?` | `string | null` |
| `Lines` | `ICollection<TaxQuoteLineRequestDto>` | `TaxQuoteLineRequestDto[]` |

## Used By Endpoints

- [POST Finalize](../tax-calculation/post-finalize-api-v1-tax-finalize.md)
- [POST Quote](../tax-calculation/post-quote-api-v1-tax-quote.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

