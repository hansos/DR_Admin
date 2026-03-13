# POST CreateFtpAccount

POST CreateFtpAccount

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/ftp
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `dto` | Body | [HostingFtpAccountCreateDto](../dtos/hosting-ftp-account-create-dto.md) |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [HostingFtpAccountDto](../dtos/hosting-ftp-account-dto.md) |

[Back to API Manual index](../index.md)




