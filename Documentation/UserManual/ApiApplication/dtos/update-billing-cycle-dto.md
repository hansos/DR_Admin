# UpdateBillingCycleDto

Data transfer object for updating an existing billing cycle

## Source

`DR_Admin/DTOs/BillingCycleDto.cs`

## TypeScript Interface

```ts
export interface UpdateBillingCycleDto {
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

- [PUT UpdateBillingCycle](../billing-cycles/put-update-billing-cycle-api-v1-billing-cycles-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

