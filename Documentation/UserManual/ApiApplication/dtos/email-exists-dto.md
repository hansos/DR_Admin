# EmailExistsDto

Data transfer object for email existence check response

## Source

`DR_Admin/DTOs/CustomerDto.cs`

## TypeScript Interface

```ts
export interface EmailExistsDto {
  email: string;
  exists: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Email` | `string` | `string` |
| `Exists` | `bool` | `boolean` |

## Used By Endpoints

- [GET CheckEmailExists](../customers/get-check-email-exists-api-v1-customers-check-email.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

