# UpdateCommunicationMessageReadStateDto

Represents the payload used to update a communication message read state.

## Source

`DR_Admin/DTOs/CommunicationThreadDto.cs`

## TypeScript Interface

```ts
export interface UpdateCommunicationMessageReadStateDto {
  isRead: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `IsRead` | `bool` | `boolean` |

## Used By Endpoints

- [PATCH UpdateMessageReadState](../communication-threads/patch-update-message-read-state-api-v1-communication-threads-messages-messageid-int-read-state.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
