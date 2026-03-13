# ServerTypeDto

Data transfer object representing a server type

## Source

`DR_Admin/DTOs/ServerTypeDto.cs`

## TypeScript Interface

```ts
export interface ServerTypeDto {
  id: number;
  name: string;
  displayName: string;
  description: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `DisplayName` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
