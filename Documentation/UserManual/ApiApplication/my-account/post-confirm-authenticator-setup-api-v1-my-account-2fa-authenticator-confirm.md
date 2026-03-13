# POST ConfirmAuthenticatorSetup

Confirms Microsoft Authenticator setup using a current verification code.

## Endpoint

```
POST /api/v1/my-account/2fa/authenticator/confirm
```

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [ConfirmAuthenticatorSetupRequestDto](../dtos/confirm-authenticator-setup-request-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




