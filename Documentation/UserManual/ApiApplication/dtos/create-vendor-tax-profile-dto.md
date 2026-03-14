# CreateVendorTaxProfileDto

Data transfer object for creating vendor tax profiles

## Source

`DR_Admin/DTOs/CreateVendorTaxProfileDto.cs`

## TypeScript Interface

```ts
export interface CreateVendorTaxProfileDto {
  vendorId: number;
  vendorType: VendorType;
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
| `VendorId` | `int` | `number` |
| `VendorType` | `VendorType` | `VendorType` |
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

- [POST CreateVendorTaxProfile](../vendor-tax-profiles/post-create-vendor-tax-profile-api-v1-vendor-tax-profiles.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

