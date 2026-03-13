# UpdateRegistrarSelectionPreferenceDto

DTO for updating existing registrar selection preference

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface UpdateRegistrarSelectionPreferenceDto {
  priority: number;
  offersHosting: boolean;
  offersEmail: boolean;
  offersSsl: boolean;
  maxCostDifferenceThreshold: number | null;
  preferForHostingCustomers: boolean;
  preferForEmailCustomers: boolean;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Priority` | `int` | `number` |
| `OffersHosting` | `bool` | `boolean` |
| `OffersEmail` | `bool` | `boolean` |
| `OffersSsl` | `bool` | `boolean` |
| `MaxCostDifferenceThreshold` | `decimal?` | `number | null` |
| `PreferForHostingCustomers` | `bool` | `boolean` |
| `PreferForEmailCustomers` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
