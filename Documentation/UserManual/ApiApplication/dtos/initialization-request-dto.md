# InitializationRequestDto

Data transfer object for system initialization request with first admin user credentials

## Source

`DR_Admin/DTOs/InitializationRequestDto.cs`

## TypeScript Interface

```ts
export interface InitializationRequestDto {
  username: string;
  password: string;
  email: string;
  companyName: string | null;
  companyEmail: string | null;
  companyPhone: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Username` | `string` | `string` |
| `Password` | `string` | `string` |
| `Email` | `string` | `string` |
| `CompanyName` | `string?` | `string | null` |
| `CompanyEmail` | `string?` | `string | null` |
| `CompanyPhone` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
