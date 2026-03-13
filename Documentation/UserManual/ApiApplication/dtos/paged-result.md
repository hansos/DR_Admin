# PagedResult

Generic wrapper for paginated results

## Source

`DR_Admin/DTOs/PagedResult.cs`

## TypeScript Interface

```ts
export interface PagedResult {
  data: T[];
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Data` | `List<T>` | `T[]` |
| `CurrentPage` | `int` | `number` |
| `PageSize` | `int` | `number` |
| `TotalCount` | `int` | `number` |
| `TotalPages` | `int` | `number` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
