# BillingCycleDto

Data transfer object representing a billing cycle

## Source

`DR_Admin/DTOs/BillingCycleDto.cs`

## TypeScript Interface

```ts
export interface BillingCycleDto {
  id: number;
  code: string;
  name: string;
  durationInDays: number;
  description: string;
  sortOrder: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property         | C# Type    | TypeScript Type |
| ---------------- | ---------- | --------------- |
| `Id`             | `int`      | `number`        |
| `Code`           | `string`   | `string`        |
| `Name`           | `string`   | `string`        |
| `DurationInDays` | `int`      | `number`        |
| `Description`    | `string`   | `string`        |
| `SortOrder`      | `int`      | `number`        |
| `CreatedAt`      | `DateTime` | `string`        |
| `UpdatedAt`      | `DateTime` | `string`        |

## Used By Endpoints

- [GET GetBillingCycleById](../billing-cycles/get-get-billing-cycle-by-id-api-v1-billing-cycles-id.md)
- [POST CreateBillingCycle](../billing-cycles/post-create-billing-cycle-api-v1-billing-cycles.md)
- [PUT UpdateBillingCycle](../billing-cycles/put-update-billing-cycle-api-v1-billing-cycles-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
