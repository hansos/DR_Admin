# RegisterAccountRequestDto

Request DTO for new account registration

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface RegisterAccountRequestDto {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  siteCode: string | null;
  isSelfRegisteredCustomer: boolean;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  customerAddress: string;
  contactFirstName: string;
  contactLastName: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Username` | `string` | `string` |
| `Email` | `string` | `string` |
| `Password` | `string` | `string` |
| `ConfirmPassword` | `string` | `string` |
| `SiteCode` | `string?` | `string | null` |
| `IsSelfRegisteredCustomer` | `bool` | `boolean` |
| `CustomerName` | `string` | `string` |
| `CustomerEmail` | `string` | `string` |
| `CustomerPhone` | `string` | `string` |
| `CustomerAddress` | `string` | `string` |
| `ContactFirstName` | `string` | `string` |
| `ContactLastName` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
