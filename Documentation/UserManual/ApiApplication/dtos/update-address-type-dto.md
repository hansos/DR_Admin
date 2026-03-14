# UpdateAddressTypeDto

Data transfer object for updating an existing address type

## Source

`DR_Admin/DTOs/AddressTypeDto.cs`

## TypeScript Interface

```ts
export interface UpdateAddressTypeDto {
  code: string;
  name: string;
  description: string | null;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |

## Used By Endpoints

- [PUT UpdateAddressType](../address-types/put-update-address-type-api-v1-address-types-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

