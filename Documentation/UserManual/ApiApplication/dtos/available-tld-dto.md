# AvailableTldDto

DTO for listing available TLDs

## Source

`DR_Admin/DTOs/DomainRegistrationDto.cs`

## TypeScript Interface

```ts
export interface AvailableTldDto {
  id: number;
  tld: string;
  registrarId: number;
  registrarName: string;
  registrationPrice: number;
  renewalPrice: number;
  currency: string;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Tld` | `string` | `string` |
| `RegistrarId` | `int` | `number` |
| `RegistrarName` | `string` | `string` |
| `RegistrationPrice` | `decimal` | `number` |
| `RenewalPrice` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

