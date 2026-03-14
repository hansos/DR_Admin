# HostingFtpAccountUpdateDto

Data transfer object for HostingFtpAccountUpdateDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingFtpAccountUpdateDto {
  homeDirectory: string | null;
  quotaMB: number | null;
  readOnly: boolean | null;
  sftpEnabled: boolean | null;
  ftpsEnabled: boolean | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HomeDirectory` | `string?` | `string | null` |
| `QuotaMB` | `int?` | `number | null` |
| `ReadOnly` | `bool?` | `boolean | null` |
| `SftpEnabled` | `bool?` | `boolean | null` |
| `FtpsEnabled` | `bool?` | `boolean | null` |

## Used By Endpoints

- [PUT UpdateFtpAccount](../hosting-ftp/put-update-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

