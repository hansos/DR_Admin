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

## Used By Endpoints

- [GET GetDefaultMailAddress](../registrar-mail-addresses/get-get-default-mail-address-api-v1-customers-customerid-registrar-mail-addresses-default.md)
- [GET GetRegistrarMailAddressById](../registrar-mail-addresses/get-get-registrar-mail-address-by-id-api-v1-customers-customerid-registrar-mail-addresses-id.md)
- [GET GetRegistrarMailAddresses](../registrar-mail-addresses/get-get-registrar-mail-addresses-api-v1-customers-customerid-registrar-mail-addresses.md)
- [POST CreateRegistrarMailAddress](../registrar-mail-addresses/post-create-registrar-mail-address-api-v1-customers-customerid-registrar-mail-addresses.md)
- [PUT SetDefaultMailAddress](../registrar-mail-addresses/put-set-default-mail-address-api-v1-customers-customerid-registrar-mail-addresses-id-set-default.md)
- [PUT UpdateRegistrarMailAddress](../registrar-mail-addresses/put-update-registrar-mail-address-api-v1-customers-customerid-registrar-mail-addresses-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

