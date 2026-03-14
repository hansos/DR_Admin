# RegistrarSelectionPreferenceDto

DTO for displaying registrar selection preference information

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface RegistrarSelectionPreferenceDto {
  id: number;
  registrarId: number;
  registrarName: string | null;
  priority: number;
  offersHosting: boolean;
  offersEmail: boolean;
  offersSsl: boolean;
  maxCostDifferenceThreshold: number | null;
  preferForHostingCustomers: boolean;
  preferForEmailCustomers: boolean;
  isActive: boolean;
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
| `Priority` | `int` | `number` |
| `OffersHosting` | `bool` | `boolean` |
| `OffersEmail` | `bool` | `boolean` |
| `OffersSsl` | `bool` | `boolean` |
| `MaxCostDifferenceThreshold` | `decimal?` | `number | null` |
| `PreferForHostingCustomers` | `bool` | `boolean` |
| `PreferForEmailCustomers` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

