# AuthenticatorSetupDto

Response DTO for authenticator app setup details.

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface AuthenticatorSetupDto {
  sharedKey: string;
  qrCodeUri: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `SharedKey` | `string` | `string` |
| `QrCodeUri` | `string` | `string` |

## Used By Endpoints

- [POST BeginAuthenticatorSetup](../my-account/post-begin-authenticator-setup-api-v1-my-account-2fa-authenticator-setup.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

