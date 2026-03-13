# CustomerTaxProfileDto

Data transfer object representing customer tax identification and validation information

## Source

`DR_Admin/DTOs/CustomerTaxProfileDto.cs`

## TypeScript Interface

```ts
export interface CustomerTaxProfileDto {
  id: number;
  customerId: number;
  taxIdNumber: string | null;
  taxIdType: TaxIdType;
  taxIdValidated: boolean;
  taxIdValidationDate: string | null;
  taxResidenceCountry: string;
  customerType: CustomerType;
  taxExempt: boolean;
  taxExemptionReason: string | null;
  taxExemptionCertificateUrl: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `TaxIdNumber` | `string?` | `string | null` |
| `TaxIdType` | `TaxIdType` | `TaxIdType` |
| `TaxIdValidated` | `bool` | `boolean` |
| `TaxIdValidationDate` | `DateTime?` | `string | null` |
| `TaxResidenceCountry` | `string` | `string` |
| `CustomerType` | `CustomerType` | `CustomerType` |
| `TaxExempt` | `bool` | `boolean` |
| `TaxExemptionReason` | `string?` | `string | null` |
| `TaxExemptionCertificateUrl` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
