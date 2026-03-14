# CreateCommunicationReplyDto

Represents the payload used to queue a reply from a communication thread.

## Source

`DR_Admin/DTOs/CommunicationThreadDto.cs`

## TypeScript Interface

```ts
export interface CreateCommunicationReplyDto {
  to: string;
  cc: string | null;
  bcc: string | null;
  subject: string | null;
  bodyText: string | null;
  bodyHtml: string | null;
  provider: string | null;
  attachmentPaths: string[] | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `To` | `string` | `string` |
| `Cc` | `string?` | `string | null` |
| `Bcc` | `string?` | `string | null` |
| `Subject` | `string?` | `string | null` |
| `BodyText` | `string?` | `string | null` |
| `BodyHtml` | `string?` | `string | null` |
| `Provider` | `string?` | `string | null` |
| `AttachmentPaths` | `List<string>?` | `string[] | null` |

## Used By Endpoints

- [POST QueueReply](../communication-threads/post-queue-reply-api-v1-communication-threads-id-int-reply.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
