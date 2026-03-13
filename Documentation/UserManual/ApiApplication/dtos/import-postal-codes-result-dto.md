# ImportPostalCodesResultDto

Result of postal codes import operation

## Source

`DR_Admin/DTOs/PostalCodeDto.cs`

## TypeScript Interface

```ts
export interface ImportPostalCodesResultDto {
  totalProcessed: number;
  created: number;
  updated: number;
  skipped: number;
  errors: string[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TotalProcessed` | `int` | `number` |
| `Created` | `int` | `number` |
| `Updated` | `int` | `number` |
| `Skipped` | `int` | `number` |
| `Errors` | `List<string>` | `string[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
