# UpdateRegistrarTldDto

Data transfer object for updating an existing registrar-TLD offering

## Source

`DR_Admin/DTOs/RegistrarTldDto.cs`

## TypeScript Interface

```ts
export interface UpdateRegistrarTldDto {
  registrarId: number;
  tldId: number;
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
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RegistrarId` | `int` | `number` |
| `TldId` | `int` | `number` |
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

## Used By Endpoints

- [PUT UpdateRegistrarTld](../registrar-tlds/put-update-registrar-tld-api-v1-registrar-tlds-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

