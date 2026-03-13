# HostingEmailAccountUpdateDto

Data transfer object for HostingEmailAccountUpdateDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingEmailAccountUpdateDto {
  quotaMB: number | null;
  forwardTo: string | null;
  autoResponderEnabled: boolean | null;
  autoResponderMessage: string | null;
  spamFilterEnabled: boolean | null;
  spamScoreThreshold: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `QuotaMB` | `int?` | `number | null` |
| `ForwardTo` | `string?` | `string | null` |
| `AutoResponderEnabled` | `bool?` | `boolean | null` |
| `AutoResponderMessage` | `string?` | `string | null` |
| `SpamFilterEnabled` | `bool?` | `boolean | null` |
| `SpamScoreThreshold` | `int?` | `number | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
