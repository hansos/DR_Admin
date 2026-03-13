# PUT UpdateEmailAccount

PUT UpdateEmailAccount

## Endpoint

```
PUT /api/v1/hosting-accounts/{hostingAccountId}/emails/{id}
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `id` | Route | `int` |
| `dto` | Body | `HostingEmailAccountUpdateDto` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `HostingEmailAccountDto` |

[Back to API Manual index](../index.md)
