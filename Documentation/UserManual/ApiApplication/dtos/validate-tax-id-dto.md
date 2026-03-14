# ValidateTaxIdDto

Data transfer object for tax ID validation requests

## Source

`DR_Admin/DTOs/ValidateTaxIdDto.cs`

## TypeScript Interface

```ts
export interface ValidateTaxIdDto {
  customerTaxProfileId: number;
  forceRevalidation: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerTaxProfileId` | `int` | `number` |
| `ForceRevalidation` | `bool` | `boolean` |

## Used By Endpoints

- [POST ValidateTaxId](../customer-tax-profiles/post-validate-tax-id-api-v1-customer-tax-profiles-validate.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

