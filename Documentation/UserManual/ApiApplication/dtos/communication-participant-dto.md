# CommunicationParticipantDto

Represents a communication participant in thread responses.

## Source

`DR_Admin/DTOs/CommunicationThreadDto.cs`

## TypeScript Interface

```ts
export interface CommunicationParticipantDto {
  emailAddress: string;
  displayName: string | null;
  role: string;
  isPrimary: boolean;
}
```

## Used By Endpoints

- [GET GetThreadById](../communication-threads/get-get-thread-by-id-api-v1-communication-threads-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
