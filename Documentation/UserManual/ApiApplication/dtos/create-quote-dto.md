# CreateQuoteDto

Data transfer object for creating a new quote

## Source

`DR_Admin/DTOs/CreateQuoteDto.cs`

## TypeScript Interface

```ts
export interface CreateQuoteDto {
  customerId: number;
  validUntil: string;
  notes: string;
  termsAndConditions: string;
  internalComment: string;
  couponCode: string | null;
  lines: CreateQuoteLineDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `ValidUntil` | `DateTime` | `string` |
| `Notes` | `string` | `string` |
| `TermsAndConditions` | `string` | `string` |
| `InternalComment` | `string` | `string` |
| `CouponCode` | `string?` | `string | null` |
| `Lines` | `List<CreateQuoteLineDto>` | `CreateQuoteLineDto[]` |

## Used By Endpoints

- [POST CreateQuote](../quotes/post-create-quote-api-v1-quotes.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

