# UpdateTwoFactorSettingsRequestDto

Request DTO for updating two-factor authentication settings.

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface UpdateTwoFactorSettingsRequestDto {
  enabled: boolean;
  method: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Enabled` | `bool` | `boolean` |
| `Method` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
