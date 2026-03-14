# CreateSentEmailDto

Data transfer object for creating a new sent email record

## Source

`DR_Admin/DTOs/SentEmailDto.cs`

## TypeScript Interface

```ts
export interface CreateSentEmailDto {
  sentDate: string;
  from: string;
  to: string;
  cc: string | null;
  bcc: string | null;
  subject: string;
  bodyText: string | null;
  bodyHtml: string | null;
  messageId: string;
  status: string | null;
  errorMessage: string | null;
  retryCount: number;
  customerId: number | null;
  userId: number | null;
  relatedEntityType: string | null;
  relatedEntityId: number | null;
  attachments: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `SentDate` | `DateTime` | `string` |
| `From` | `string` | `string` |
| `To` | `string` | `string` |
| `Cc` | `string?` | `string | null` |
| `Bcc` | `string?` | `string | null` |
| `Subject` | `string` | `string` |
| `BodyText` | `string?` | `string | null` |
| `BodyHtml` | `string?` | `string | null` |
| `MessageId` | `string` | `string` |
| `Status` | `string?` | `string | null` |
| `ErrorMessage` | `string?` | `string | null` |
| `RetryCount` | `int` | `number` |
| `CustomerId` | `int?` | `number | null` |
| `UserId` | `int?` | `number | null` |
| `RelatedEntityType` | `string?` | `string | null` |
| `RelatedEntityId` | `int?` | `number | null` |
| `Attachments` | `string?` | `string | null` |

## Used By Endpoints

- [POST CreateSentEmail](../sent-emails/post-create-sent-email-api-v1-sent-emails.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

