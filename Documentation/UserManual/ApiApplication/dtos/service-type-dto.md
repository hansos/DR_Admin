# ServiceTypeDto

Data transfer object representing a service type category

## Source

`DR_Admin/DTOs/ServiceTypeDto.cs`

## TypeScript Interface

```ts
export interface ServiceTypeDto {
  id: number;
  name: string;
  description: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetServiceTypeById](../service-types/get-get-service-type-by-id-api-v1-service-types-id.md)
- [GET GetServiceTypeByName](../service-types/get-get-service-type-by-name-api-v1-service-types-by-name-name.md)
- [POST CreateServiceType](../service-types/post-create-service-type-api-v1-service-types.md)
- [PUT UpdateServiceType](../service-types/put-update-service-type-api-v1-service-types-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

