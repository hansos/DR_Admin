# CreateBillingCycleDto

Data transfer object for creating a new billing cycle

## Source

`DR_Admin/DTOs/BillingCycleDto.cs`

## TypeScript Interface

```ts
export interface CreateBillingCycleDto {
  code: string;
  name: string;
  durationInDays: number;
  description: string;
  sortOrder: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `DurationInDays` | `int` | `number` |
| `Description` | `string` | `string` |
| `SortOrder` | `int` | `number` |

## Used By Endpoints

- [POST CreateBillingCycle](../billing-cycles/post-create-billing-cycle-api-v1-billing-cycles.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

