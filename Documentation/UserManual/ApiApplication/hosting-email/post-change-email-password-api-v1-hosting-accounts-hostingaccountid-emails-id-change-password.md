# POST ChangeEmailPassword

POST ChangeEmailPassword

## Endpoint

```
POST /api/v1/hosting-accounts/{hostingAccountId}/emails/{id}/change-password
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `id` | Route | `int` |
| `newPassword` | Body | `string` |
| `syncToServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |

[Back to API Manual index](../index.md)
