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
| `dto` | Body | `HostingFtpAccountCreateDto` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `HostingFtpAccountDto` |

[Back to API Manual index](../index.md)
