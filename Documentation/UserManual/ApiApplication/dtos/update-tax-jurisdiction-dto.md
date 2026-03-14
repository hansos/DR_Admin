# UpdateTaxJurisdictionDto

Data transfer object for updating a tax jurisdiction.

## Source

`DR_Admin/DTOs/UpdateTaxJurisdictionDto.cs`

## TypeScript Interface

```ts
export interface UpdateTaxJurisdictionDto {
  code: string;
  name: string;
  countryCode: string;
  stateCode: string | null;
  taxAuthority: string;
  taxCurrencyCode: string;
  isActive: boolean;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `CountryCode` | `string` | `string` |
| `StateCode` | `string?` | `string | null` |
| `TaxAuthority` | `string` | `string` |
| `TaxCurrencyCode` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [PUT UpdateTaxJurisdiction](../tax-jurisdictions/put-update-tax-jurisdiction-api-v1-tax-jurisdictions-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

