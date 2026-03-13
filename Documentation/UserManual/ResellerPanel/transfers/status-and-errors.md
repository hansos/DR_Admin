# Status and Errors

Monitor transfer status and troubleshoot transfer failures with detailed error information.

## How to Access

Navigate to **Transfers > Status & Errors** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Domain Name | The domain involved in the transfer. |
| Direction | Incoming or Outgoing. |
| Status | Current status of the transfer. |
| Error Code | Registry error code (if failed). |
| Error Message | Human-readable error description. |
| Date | Date of the last status update. |
| Actions | Retry, Cancel, View Details. |

## Common Errors

| Error | Meaning | Resolution |
|-------|---------|------------|
| Transfer Locked | Domain has a transfer lock enabled. | Unlock the domain in [Domain Details](../domains/domain-details.md). |
| Invalid Auth Code | The authorization code is wrong or expired. | Generate a new code in [Auth Codes](auth-codes.md). |
| Domain Recently Transferred | 60-day transfer lock after a previous transfer. | Wait for the lock period to expire. |
| Registrant Denied | Registrant did not approve the transfer. | Contact the registrant and retry. |

## Related Pages

- [Incoming Transfers](incoming-transfers.md)
- [Outgoing Transfers](outgoing-transfers.md)
- [Auth Codes](auth-codes.md)

[Back to Reseller Manual index](../index.md)
