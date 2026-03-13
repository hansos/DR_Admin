# QueueEmailDto

DTO for queuing an email to be sent

## Source

`DR_Admin/DTOs/QueueEmailDto.cs`

## TypeScript Interface

```ts
export interface QueueEmailDto {
  to: string;
  cc: string | null;
  bcc: string | null;
  subject: string;
  bodyText: string | null;
  bodyHtml: string | null;
  provider: string | null;
  customerId: number | null;
  userId: number | null;
  relatedEntityType: string | null;
  relatedEntityId: number | null;
  attachmentPaths: string[] | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `To` | `string` | `string` |
| `Cc` | `string?` | `string | null` |
| `Bcc` | `string?` | `string | null` |
| `Subject` | `string` | `string` |
| `BodyText` | `string?` | `string | null` |
| `BodyHtml` | `string?` | `string | null` |
| `Provider` | `string?` | `string | null` |
| `CustomerId` | `int?` | `number | null` |
| `UserId` | `int?` | `number | null` |
| `RelatedEntityType` | `string?` | `string | null` |
| `RelatedEntityId` | `int?` | `number | null` |
| `AttachmentPaths` | `List<string>?` | `string[] | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
