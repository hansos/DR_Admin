# CreateTaxRegistrationDto

Data transfer object for creating a seller tax registration.

## Source

`DR_Admin/DTOs/CreateTaxRegistrationDto.cs`

## TypeScript Interface

```ts
export interface CreateTaxRegistrationDto {
  taxJurisdictionId: number;
  legalEntityName: string;
  registrationNumber: string;
  effectiveFrom: string;
  effectiveUntil: string | null;
  isActive: boolean;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TaxJurisdictionId` | `int` | `number` |
| `LegalEntityName` | `string` | `string` |
| `RegistrationNumber` | `string` | `string` |
| `EffectiveFrom` | `DateTime` | `string` |
| `EffectiveUntil` | `DateTime?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
