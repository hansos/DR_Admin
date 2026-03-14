# TldRegistryRuleDto

Data transfer object representing TLD registry policy rules.

## Source

`DR_Admin/DTOs/TldRegistryRuleDto.cs`

## TypeScript Interface

```ts
export interface TldRegistryRuleDto {
  id: number;
  tldId: number;
  tldExtension: string;
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
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `TldId` | `int` | `number` |
| `TldExtension` | `string` | `string` |
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
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetById](../tld-registry-rules/get-get-by-id-api-v1-tld-registry-rules-id-int.md)
- [POST Create](../tld-registry-rules/post-create-api-v1-tld-registry-rules.md)
- [PUT Update](../tld-registry-rules/put-update-api-v1-tld-registry-rules-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

