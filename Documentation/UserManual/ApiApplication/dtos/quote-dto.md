# QuoteDto

Data transfer object representing a quote

## Source

`DR_Admin/DTOs/QuoteDto.cs`

## TypeScript Interface

```ts
export interface QuoteDto {
  id: number;
  quoteNumber: string;
  customerId: number;
  customerName: string;
  status: QuoteStatus;
  validUntil: string;
  subTotal: number;
  totalSetupFee: number;
  totalRecurring: number;
  taxAmount: number;
  totalAmount: number;
  currencyCode: string;
  taxRate: number;
  taxName: string;
  customerAddress: string;
  customerTaxId: string;
  notes: string;
  termsAndConditions: string;
  internalComment: string;
  sentAt: string | null;
  acceptedAt: string | null;
  rejectedAt: string | null;
  rejectionReason: string;
  preparedByUserId: number | null;
  couponId: number | null;
  discountAmount: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `QuoteNumber` | `string` | `string` |
| `CustomerId` | `int` | `number` |
| `CustomerName` | `string` | `string` |
| `Status` | `QuoteStatus` | `QuoteStatus` |
| `ValidUntil` | `DateTime` | `string` |
| `SubTotal` | `decimal` | `number` |
| `TotalSetupFee` | `decimal` | `number` |
| `TotalRecurring` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `TotalAmount` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `TaxRate` | `decimal` | `number` |
| `TaxName` | `string` | `string` |
| `CustomerAddress` | `string` | `string` |
| `CustomerTaxId` | `string` | `string` |
| `Notes` | `string` | `string` |
| `TermsAndConditions` | `string` | `string` |
| `InternalComment` | `string` | `string` |
| `SentAt` | `DateTime?` | `string | null` |
| `AcceptedAt` | `DateTime?` | `string | null` |
| `RejectedAt` | `DateTime?` | `string | null` |
| `RejectionReason` | `string` | `string` |
| `PreparedByUserId` | `int?` | `number | null` |
| `CouponId` | `int?` | `number | null` |
| `DiscountAmount` | `decimal` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetQuoteById](../quotes/get-get-quote-by-id-api-v1-quotes-id.md)
- [POST CreateQuote](../quotes/post-create-quote-api-v1-quotes.md)
- [PUT UpdateQuote](../quotes/put-update-quote-api-v1-quotes-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

