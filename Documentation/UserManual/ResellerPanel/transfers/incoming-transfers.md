# Incoming Transfers

Manage domains being transferred into your reseller account from other registrars.

## How to Access

Navigate to **Transfers > Incoming Transfers** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Domain Name | The domain being transferred in. |
| Customer | The customer requesting the transfer. |
| Status | Pending, In Progress, Completed, Failed. |
| Initiated | Date the transfer was started. |
| Auth Code | Whether the authorization code has been provided. |
| Actions | View, Cancel, Retry. |

## Initiating an Incoming Transfer

1. Click **New Transfer**.
2. Enter the domain name.
3. Select or create the customer.
4. Enter the **Authorization Code** (EPP code) obtained from the current registrar.
5. Click **Submit Transfer**.

## Transfer Lifecycle

1. **Pending** â€” Request submitted, awaiting registry processing.
2. **In Progress** â€” Registry is processing; losing registrar may need to approve.
3. **Completed** â€” Domain successfully transferred to your account.
4. **Failed** â€” Transfer rejected. Check [Status & Errors](status-and-errors.md) for details.

## Related Pages

- [Auth Codes](auth-codes.md)
- [Status & Errors](status-and-errors.md)
- [Registry Rules](registry-rules.md)

[Back to Reseller Manual index](../index.md)
