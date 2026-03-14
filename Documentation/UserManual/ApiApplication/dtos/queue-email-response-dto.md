# QueueEmailResponseDto

DTO for email queue response

## Source

`DR_Admin/DTOs/QueueEmailDto.cs`

## TypeScript Interface

```ts
export interface QueueEmailResponseDto {
  id: number;
  status: string;
  message: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Status` | `string` | `string` |
| `Message` | `string` | `string` |

## Used By Endpoints

- [POST QueueEmail](../email-queue/post-queue-email-api-v1-email-queue-queue.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

