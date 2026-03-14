# CommunicationMessageDto

Represents a communication message in thread responses.

## Source

`DR_Admin/DTOs/CommunicationThreadDto.cs`

## TypeScript Interface

```ts
export interface CommunicationMessageDto {
  id: number;
  direction: string;
  externalMessageId: string | null;
  fromAddress: string;
  toAddresses: string;
  ccAddresses: string | null;
  bccAddresses: string | null;
  subject: string;
  bodyText: string | null;
  bodyHtml: string | null;
  provider: string | null;
  isRead: boolean;
  receivedAtUtc: string | null;
  sentAtUtc: string | null;
  readAtUtc: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Used By Endpoints

- [GET GetThreadById](../communication-threads/get-get-thread-by-id-api-v1-communication-threads-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
