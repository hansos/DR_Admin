# RegistrarMailAddressDto

Data transfer object representing a registrar mail address

## Source

`DR_Admin/DTOs/RegistrarMailAddressDto.cs`

## TypeScript Interface

```ts
export interface RegistrarMailAddressDto {
  id: number;
  customerId: number;
  mailAddress: string;
  isDefault: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `MailAddress` | `string` | `string` |
| `IsDefault` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
