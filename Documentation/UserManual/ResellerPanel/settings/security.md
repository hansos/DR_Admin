# Security

Configure security settings including authentication policies, session timeouts, and access controls.

## How to Access

Navigate to **Settings > Security** from the side menu.

## Configuration Sections

### Password Policy

| Setting | Description |
|---------|-------------|
| Minimum Length | Minimum number of characters required. |
| Complexity | Require uppercase, lowercase, digits, special characters. |
| Expiry (days) | Force password change after a set number of days (0 = never). |
| History | Number of previous passwords that cannot be reused. |

### Session Settings

| Setting | Description |
|---------|-------------|
| Session Timeout | Idle time before automatic logout (minutes). |
| Max Concurrent Sessions | Maximum simultaneous sessions per user. |

### Account Lockout

| Setting | Description |
|---------|-------------|
| Max Failed Attempts | Number of failed login attempts before lockout. |
| Lockout Duration | How long the account is locked (minutes). |

### Two-Factor Authentication (2FA)

| Setting | Description |
|---------|-------------|
| Enforce 2FA | Require all users to enable 2FA. |
| Allowed Methods | TOTP (authenticator app), SMS, Email. |

## Saving Changes

Click **Save** to apply. Changes take effect for all users on their next login.

## Related Pages

- [User Management](../users/user-management.md)
- [Login History](../users/login-history.md)
- [Change Password](../dashboard/change-password.md)

[Back to Reseller Manual index](../index.md)
