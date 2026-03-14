# CreateRegistrarMailAddressDto

Data transfer object for creating a new registrar mail address

## Source

`DR_Admin/DTOs/RegistrarMailAddressDto.cs`

## TypeScript Interface

```ts
export interface CreateRegistrarMailAddressDto {
  mailAddress: string;
  isDefault: boolean;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `MailAddress` | `string` | `string` |
| `IsDefault` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [POST CreateRegistrarMailAddress](../registrar-mail-addresses/post-create-registrar-mail-address-api-v1-customers-customerid-registrar-mail-addresses.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

