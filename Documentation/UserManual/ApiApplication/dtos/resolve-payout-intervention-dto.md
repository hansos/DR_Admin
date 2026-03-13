# ResolvePayoutInterventionDto

Data transfer object for resolving vendor payout manual interventions

## Source

`DR_Admin/DTOs/ResolvePayoutInterventionDto.cs`

## TypeScript Interface

```ts
export interface ResolvePayoutInterventionDto {
  vendorPayoutId: number;
  resolvedByUserId: number;
  resolutionNotes: string;
  proceedWithPayout: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `VendorPayoutId` | `int` | `number` |
| `ResolvedByUserId` | `int` | `number` |
| `ResolutionNotes` | `string` | `string` |
| `ProceedWithPayout` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
