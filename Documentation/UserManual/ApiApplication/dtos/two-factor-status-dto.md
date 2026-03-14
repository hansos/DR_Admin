# TwoFactorStatusDto

DTO for two-factor authentication status.

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface TwoFactorStatusDto {
  enabled: boolean;
  method: string | null;
  recoveryCodesRemaining: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Enabled` | `bool` | `boolean` |
| `Method` | `string?` | `string | null` |
| `RecoveryCodesRemaining` | `int?` | `number | null` |

## Used By Endpoints

- [GET GetTwoFactorStatus](../my-account/get-get-two-factor-status-api-v1-my-account-2fa-status.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

