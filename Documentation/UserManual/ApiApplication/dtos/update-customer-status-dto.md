# UpdateCustomerStatusDto

Data transfer object for updating an existing customer status

## Source

`DR_Admin/DTOs/CustomerStatusDto.cs`

## TypeScript Interface

```ts
export interface UpdateCustomerStatusDto {
  name: string;
  description: string | null;
  color: string | null;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
  priority: number | null;
  isSystem: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `Color` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |
| `Priority` | `int?` | `number | null` |
| `IsSystem` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdateCustomerStatus](../customer-statuses/put-update-customer-status-api-v1-customer-statuses-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

