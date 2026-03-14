# CommunicationThreadDetailsDto

Represents detailed communication thread data including messages and participants.

## Source

`DR_Admin/DTOs/CommunicationThreadDto.cs`

## TypeScript Interface

```ts
export interface CommunicationThreadDetailsDto extends CommunicationThreadDto {
  participants: CommunicationParticipantDto[];
  messages: CommunicationMessageDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Participants` | `List<CommunicationParticipantDto>` | `CommunicationParticipantDto[]` |
| `Messages` | `List<CommunicationMessageDto>` | `CommunicationMessageDto[]` |

## Used By Endpoints

- [GET GetThreadById](../communication-threads/get-get-thread-by-id-api-v1-communication-threads-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
