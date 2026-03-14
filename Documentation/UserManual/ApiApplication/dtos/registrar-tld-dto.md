# RegistrarTldDto

Data transfer object representing a registrar's TLD offering with pricing information

## Source

`DR_Admin/DTOs/RegistrarTldDto.cs`

## TypeScript Interface

```ts
export interface RegistrarTldDto {
  id: number;
  registrarId: number;
  registrarName: string | null;
  tldId: number;
  tldExtension: string | null;
  registrationCost: number;
  registrationPrice: number;
  renewalCost: number;
  renewalPrice: number;
  transferCost: number;
  transferPrice: number;
  privacyCost: number | null;
  privacyPrice: number | null;
  currency: string;
  isActive: boolean;
  autoRenew: boolean;
  minRegistrationYears: number | null;
  maxRegistrationYears: number | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `RegistrarId` | `int` | `number` |
| `RegistrarName` | `string?` | `string | null` |
| `TldId` | `int` | `number` |
| `TldExtension` | `string?` | `string | null` |
| `RegistrationCost` | `decimal` | `number` |
| `RegistrationPrice` | `decimal` | `number` |
| `RenewalCost` | `decimal` | `number` |
| `RenewalPrice` | `decimal` | `number` |
| `TransferCost` | `decimal` | `number` |
| `TransferPrice` | `decimal` | `number` |
| `PrivacyCost` | `decimal?` | `number | null` |
| `PrivacyPrice` | `decimal?` | `number | null` |
| `Currency` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `AutoRenew` | `bool` | `boolean` |
| `MinRegistrationYears` | `int?` | `number | null` |
| `MaxRegistrationYears` | `int?` | `number | null` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [POST AssignTldToRegistrarByDto](../registrars/post-assign-tld-to-registrar-by-dto-api-v1-registrars-registrarid-tld.md)
- [POST AssignTldToRegistrarByIds](../registrars/post-assign-tld-to-registrar-by-ids-api-v1-registrars-registrarid-tld-tldid.md)
- [GET GetAllRegistrarTlds](../registrar-tlds/get-get-all-registrar-tlds-api-v1-registrar-tlds.md)
- [GET GetRegistrarTldById](../registrar-tlds/get-get-registrar-tld-by-id-api-v1-registrar-tlds-id.md)
- [GET GetRegistrarTldByRegistrarAndTld](../registrar-tlds/get-get-registrar-tld-by-registrar-and-tld-api-v1-registrar-tlds-registrar-registrarid-tld-tldid.md)
- [GET GetRegistrarTldsByRegistrar](../registrar-tlds/get-get-registrar-tlds-by-registrar-api-v1-registrar-tlds-registrar-registrarid.md)
- [POST CreateRegistrarTld](../registrar-tlds/post-create-registrar-tld-api-v1-registrar-tlds.md)
- [PUT UpdateRegistrarTld](../registrar-tlds/put-update-registrar-tld-api-v1-registrar-tlds-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

