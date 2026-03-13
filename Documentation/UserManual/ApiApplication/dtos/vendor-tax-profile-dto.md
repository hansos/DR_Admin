# VendorTaxProfileDto

Data transfer object representing vendor tax information

## Source

`DR_Admin/DTOs/VendorTaxProfileDto.cs`

## TypeScript Interface

```ts
export interface VendorTaxProfileDto {
  id: number;
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
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
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
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
