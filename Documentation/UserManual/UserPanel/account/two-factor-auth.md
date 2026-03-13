# Two-Factor Authentication

Manage additional sign-in verification and email verification status.

## How to Access

Navigate to **Account > Two-Factor Auth** from the side menu.

## Page Layout

### Account Security Summary

Displays your current security status, including:
- Whether two-factor authentication is enabled.
- Email verification status.
- Option to send a verification email if your email is not yet verified.

### TOTP Setup

If 2FA is not yet enabled:

1. Click **Enable 2FA**.
2. Scan the QR code with your authenticator app (e.g., Google Authenticator, Microsoft Authenticator).
3. Enter the verification code from the app to confirm.
4. Save the recovery codes in a secure location.

If 2FA is already enabled:
- **Disable 2FA** — Turn off two-factor authentication (requires current verification code).
- **Regenerate recovery codes** — Generate a new set of backup codes.

## Important Notes

- Once enabled, you will be prompted for a verification code on every sign-in.
- Keep your recovery codes safe — they are the only way to regain access if you lose your authenticator device.

## Related Pages

- [Security](security.md)

[Back to User Panel Manual index](../index.md)
