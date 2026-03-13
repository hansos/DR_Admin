# CreateServerTypeDto

Data transfer object for creating a new server type

## Source

`DR_Admin/DTOs/ServerTypeDto.cs`

## TypeScript Interface

```ts
export interface CreateServerTypeDto {
  name: string;
  displayName: string;
  description: string | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `DisplayName` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
