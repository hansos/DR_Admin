# RegistrarCurrentCostByTldDto

DTO for displaying current registrar cost pricing rows for a specific TLD.

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface RegistrarCurrentCostByTldDto {
  registrarTldId: number;
  registrarId: number;
  registrarName: string;
  registrationCost: number | null;
  renewalCost: number | null;
  transferCost: number | null;
  currency: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RegistrarTldId` | `int` | `number` |
| `RegistrarId` | `int` | `number` |
| `RegistrarName` | `string` | `string` |
| `RegistrationCost` | `decimal?` | `number | null` |
| `RenewalCost` | `decimal?` | `number | null` |
| `TransferCost` | `decimal?` | `number | null` |
| `Currency` | `string?` | `string | null` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

