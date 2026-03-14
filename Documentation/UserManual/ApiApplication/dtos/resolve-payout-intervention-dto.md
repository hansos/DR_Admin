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

## Used By Endpoints

- [POST ResolvePayoutIntervention](../vendor-payouts/post-resolve-payout-intervention-api-v1-vendor-payouts-resolve-intervention.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

