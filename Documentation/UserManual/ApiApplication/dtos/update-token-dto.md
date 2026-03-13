# UpdateTokenDto

Data transfer object for updating an existing token (primarily for revocation)

## Source

`DR_Admin/DTOs/TokenDto.cs`

## TypeScript Interface

```ts
export interface UpdateTokenDto {
  revokedAt: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RevokedAt` | `DateTime?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
