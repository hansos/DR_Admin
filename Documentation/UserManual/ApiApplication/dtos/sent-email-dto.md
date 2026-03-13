# SentEmailDto

Data transfer object representing a sent email

## Source

`DR_Admin/DTOs/SentEmailDto.cs`

## TypeScript Interface

```ts
export interface SentEmailDto {
  id: number;
  sentDate: string | null;
  from: string;
  to: string;
  cc: string | null;
  bcc: string | null;
  subject: string;
  body: string | null;
  messageId: string;
  status: string | null;
  errorMessage: string | null;
  retryCount: number | null;
  customerId: number | null;
  userId: number | null;
  relatedEntityType: string | null;
  relatedEntityId: number | null;
  attachments: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `SentDate` | `DateTime?` | `string | null` |
| `From` | `string` | `string` |
| `To` | `string` | `string` |
| `Cc` | `string?` | `string | null` |
| `Bcc` | `string?` | `string | null` |
| `Subject` | `string` | `string` |
| `Body` | `string?` | `string | null` |
| `MessageId` | `string` | `string` |
| `Status` | `string?` | `string | null` |
| `ErrorMessage` | `string?` | `string | null` |
| `RetryCount` | `int?` | `number | null` |
| `CustomerId` | `int?` | `number | null` |
| `UserId` | `int?` | `number | null` |
| `RelatedEntityType` | `string?` | `string | null` |
| `RelatedEntityId` | `int?` | `number | null` |
| `Attachments` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
