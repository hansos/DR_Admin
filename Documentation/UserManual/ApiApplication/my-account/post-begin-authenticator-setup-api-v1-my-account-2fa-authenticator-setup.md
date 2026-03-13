# POST BeginAuthenticatorSetup

Starts Microsoft Authenticator setup by generating a shared key and QR provisioning URI.

## Endpoint

```
POST /api/v1/my-account/2fa/authenticator/setup
```

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[AuthenticatorSetupDto](../dtos/authenticator-setup-dto.md)` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



