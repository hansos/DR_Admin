# TaxRegistrationDto

Data transfer object representing a seller tax registration.

## Source

`DR_Admin/DTOs/TaxRegistrationDto.cs`

## TypeScript Interface

```ts
export interface TaxRegistrationDto {
  id: number;
  taxJurisdictionId: number;
  legalEntityName: string;
  registrationNumber: string;
  effectiveFrom: string;
  effectiveUntil: string | null;
  isActive: boolean;
  notes: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `TaxJurisdictionId` | `int` | `number` |
| `LegalEntityName` | `string` | `string` |
| `RegistrationNumber` | `string` | `string` |
| `EffectiveFrom` | `DateTime` | `string` |
| `EffectiveUntil` | `DateTime?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetTaxRegistrationById](../tax-registrations/get-get-tax-registration-by-id-api-v1-tax-registrations-id.md)
- [POST CreateTaxRegistration](../tax-registrations/post-create-tax-registration-api-v1-tax-registrations.md)
- [PUT UpdateTaxRegistration](../tax-registrations/put-update-tax-registration-api-v1-tax-registrations-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

