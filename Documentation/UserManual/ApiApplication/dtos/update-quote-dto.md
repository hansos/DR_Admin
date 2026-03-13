# UpdateQuoteDto

Data transfer object for updating an existing quote

## Source

`DR_Admin/DTOs/UpdateQuoteDto.cs`

## TypeScript Interface

```ts
export interface UpdateQuoteDto {
  validUntil: string;
  notes: string;
  termsAndConditions: string;
  internalComment: string;
  lines: UpdateQuoteLineDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ValidUntil` | `DateTime` | `string` |
| `Notes` | `string` | `string` |
| `TermsAndConditions` | `string` | `string` |
| `InternalComment` | `string` | `string` |
| `Lines` | `List<UpdateQuoteLineDto>` | `UpdateQuoteLineDto[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
