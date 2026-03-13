# PUT UpdateFtpAccount

PUT UpdateFtpAccount

## Endpoint

```
PUT /api/v1/hosting-accounts/{hostingAccountId}/ftp/{id}
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `id` | Route | `int` |
| `dto` | Body | [HostingFtpAccountUpdateDto](../dtos/hosting-ftp-account-update-dto.md) |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [HostingFtpAccountDto](../dtos/hosting-ftp-account-dto.md) |

[Back to API Manual index](../index.md)




