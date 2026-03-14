# UserPanelInitializationResponseDto

Data transfer object for user panel initialization response.

## Source

`DR_Admin/DTOs/InitializationResponseDto.cs`

## TypeScript Interface

```ts
export interface UserPanelInitializationResponseDto {
  success: boolean;
  message: string;
  userId: number;
  username: string;
  email: string;
  companyName: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `UserId` | `int` | `number` |
| `Username` | `string` | `string` |
| `Email` | `string` | `string` |
| `CompanyName` | `string` | `string` |

## Used By Endpoints

- [POST InitializeCustomer](../initialization/post-initialize-customer-api-v1-initialization-initialize-customer.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

