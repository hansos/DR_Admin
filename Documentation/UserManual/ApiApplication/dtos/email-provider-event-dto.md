# EmailProviderEventDto

Represents a provider webhook event for email delivery lifecycle updates.

## Source

`DR_Admin/DTOs/EmailProviderEventDto.cs`

## TypeScript Interface

```ts
export interface EmailProviderEventDto {
  providerMessageId: string;
  eventType: string;
  details: string | null;
  source: string | null;
  occurredAtUtc: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ProviderMessageId` | `string` | `string` |
| `EventType` | `string` | `string` |
| `Details` | `string?` | `string | null` |
| `Source` | `string?` | `string | null` |
| `OccurredAtUtc` | `DateTime?` | `string | null` |

## Used By Endpoints

- [POST ApplyProviderEvent](../email-queue/post-apply-provider-event-api-v1-email-queue-events-provider.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
