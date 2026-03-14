# InitializationResponseDto

Data transfer object for system initialization response

## Source

`DR_Admin/DTOs/InitializationResponseDto.cs`

## TypeScript Interface

```ts
export interface InitializationResponseDto {
  success: boolean;
  message: string;
  username: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `Username` | `string` | `string` |

## Used By Endpoints

- [POST InitializeAdmin](../initialization/post-initialize-admin-api-v1-initialization-initialize-admin.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

