# CreateTldRegistryRuleDto

Data transfer object for creating TLD registry policy rules.

## Source

`DR_Admin/DTOs/TldRegistryRuleDto.cs`

## TypeScript Interface

```ts
export interface CreateTldRegistryRuleDto {
  tldId: number;
  requireRegistrantContact: boolean;
  requireAdministrativeContact: boolean;
  requireTechnicalContact: boolean;
  requireBillingContact: boolean;
  requiresAuthCodeForTransfer: boolean;
  transferLockDays: number | null;
  renewalGracePeriodDays: number | null;
  redemptionGracePeriodDays: number | null;
  notes: string | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TldId` | `int` | `number` |
| `RequireRegistrantContact` | `bool` | `boolean` |
| `RequireAdministrativeContact` | `bool` | `boolean` |
| `RequireTechnicalContact` | `bool` | `boolean` |
| `RequireBillingContact` | `bool` | `boolean` |
| `RequiresAuthCodeForTransfer` | `bool` | `boolean` |
| `TransferLockDays` | `int?` | `number | null` |
| `RenewalGracePeriodDays` | `int?` | `number | null` |
| `RedemptionGracePeriodDays` | `int?` | `number | null` |
| `Notes` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
