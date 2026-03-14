# HostingFtpAccountCreateDto

Data transfer object for HostingFtpAccountCreateDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingFtpAccountCreateDto {
  hostingAccountId: number;
  username: string;
  password: string;
  homeDirectory: string;
  quotaMB: number | null;
  readOnly: boolean;
  sftpEnabled: boolean;
  ftpsEnabled: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingAccountId` | `int` | `number` |
| `Username` | `string` | `string` |
| `Password` | `string` | `string` |
| `HomeDirectory` | `string` | `string` |
| `QuotaMB` | `int?` | `number | null` |
| `ReadOnly` | `bool` | `boolean` |
| `SftpEnabled` | `bool` | `boolean` |
| `FtpsEnabled` | `bool` | `boolean` |

## Used By Endpoints

- [POST CreateFtpAccount](../hosting-ftp/post-create-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

