# CustomerStatusDto

Data transfer object representing a customer status

## Source

`DR_Admin/DTOs/CustomerStatusDto.cs`

## TypeScript Interface

```ts
export interface CustomerStatusDto {
  id: number;
  code: string;
  name: string;
  description: string | null;
  color: string | null;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
  priority: number | null;
  isSystem: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `Color` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |
| `Priority` | `int?` | `number | null` |
| `IsSystem` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetCustomerStatusByCode](../customer-statuses/get-get-customer-status-by-code-api-v1-customer-statuses-code-code.md)
- [GET GetCustomerStatusById](../customer-statuses/get-get-customer-status-by-id-api-v1-customer-statuses-id.md)
- [GET GetDefaultCustomerStatus](../customer-statuses/get-get-default-customer-status-api-v1-customer-statuses-default.md)
- [POST CreateCustomerStatus](../customer-statuses/post-create-customer-status-api-v1-customer-statuses.md)
- [PUT UpdateCustomerStatus](../customer-statuses/put-update-customer-status-api-v1-customer-statuses-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

