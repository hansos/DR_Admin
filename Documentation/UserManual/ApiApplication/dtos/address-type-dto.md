# AddressTypeDto

Data transfer object representing an address type

## Source

`DR_Admin/DTOs/AddressTypeDto.cs`

## TypeScript Interface

```ts
export interface AddressTypeDto {
  id: number;
  code: string;
  name: string;
  description: string | null;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
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
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetAddressTypeById](../address-types/get-get-address-type-by-id-api-v1-address-types-id.md)
- [GET GetAllAddressTypes](../address-types/get-get-all-address-types-api-v1-address-types.md)
- [POST CreateAddressType](../address-types/post-create-address-type-api-v1-address-types.md)
- [PUT UpdateAddressType](../address-types/put-update-address-type-api-v1-address-types-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

