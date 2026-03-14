# UpdateSentEmailDto

Data transfer object for updating an existing sent email record

## Source

`DR_Admin/DTOs/SentEmailDto.cs`

## TypeScript Interface

```ts
export interface UpdateSentEmailDto {
  status: string | null;
  errorMessage: string | null;
  retryCount: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Status` | `string?` | `string | null` |
| `ErrorMessage` | `string?` | `string | null` |
| `RetryCount` | `int` | `number` |

## Used By Endpoints

- [PUT UpdateSentEmail](../sent-emails/put-update-sent-email-api-v1-sent-emails-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

