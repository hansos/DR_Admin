# UpdateVendorTaxProfileDto

Data transfer object for updating vendor tax profiles

## Source

`DR_Admin/DTOs/UpdateVendorTaxProfileDto.cs`

## TypeScript Interface

```ts
export interface UpdateVendorTaxProfileDto {
  taxIdNumber: string | null;
  taxResidenceCountry: string;
  require1099: boolean;
  w9OnFile: boolean;
  w9FileUrl: string | null;
  withholdingTaxRate: number | null;
  taxTreatyExempt: boolean;
  taxTreatyCountry: string | null;
  taxNotes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TaxIdNumber` | `string?` | `string | null` |
| `TaxResidenceCountry` | `string` | `string` |
| `Require1099` | `bool` | `boolean` |
| `W9OnFile` | `bool` | `boolean` |
| `W9FileUrl` | `string?` | `string | null` |
| `WithholdingTaxRate` | `decimal?` | `number | null` |
| `TaxTreatyExempt` | `bool` | `boolean` |
| `TaxTreatyCountry` | `string?` | `string | null` |
| `TaxNotes` | `string` | `string` |

## Used By Endpoints

- [PUT UpdateVendorTaxProfile](../vendor-tax-profiles/put-update-vendor-tax-profile-api-v1-vendor-tax-profiles-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

