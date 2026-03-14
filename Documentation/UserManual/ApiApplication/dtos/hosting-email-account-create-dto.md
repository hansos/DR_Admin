# HostingEmailAccountCreateDto

Data transfer object for HostingEmailAccountCreateDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingEmailAccountCreateDto {
  hostingAccountId: number;
  emailAddress: string;
  username: string;
  password: string;
  quotaMB: number | null;
  isForwarderOnly: boolean;
  forwardTo: string | null;
  autoResponderEnabled: boolean;
  autoResponderMessage: string | null;
  spamFilterEnabled: boolean;
  spamScoreThreshold: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingAccountId` | `int` | `number` |
| `EmailAddress` | `string` | `string` |
| `Username` | `string` | `string` |
| `Password` | `string` | `string` |
| `QuotaMB` | `int?` | `number | null` |
| `IsForwarderOnly` | `bool` | `boolean` |
| `ForwardTo` | `string?` | `string | null` |
| `AutoResponderEnabled` | `bool` | `boolean` |
| `AutoResponderMessage` | `string?` | `string | null` |
| `SpamFilterEnabled` | `bool` | `boolean` |
| `SpamScoreThreshold` | `int?` | `number | null` |

## Used By Endpoints

- [POST CreateEmailAccount](../hosting-email/post-create-email-account-api-v1-hosting-accounts-hostingaccountid-emails.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

