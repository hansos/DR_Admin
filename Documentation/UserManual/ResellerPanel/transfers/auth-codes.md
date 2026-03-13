# Auth Codes

Generate and manage authorization (EPP) codes required for domain transfers.

## How to Access

Navigate to **Transfers > Auth Codes** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Domain Name | The domain the auth code belongs to. |
| Auth Code | The current authorization code (masked by default). |
| Generated | Date the code was generated. |
| Expires | Expiration date of the code (registry-dependent). |
| Actions | Reveal, Regenerate, Copy, Email to Customer. |

## Generating an Auth Code

1. Click **Generate** next to a domain, or use the **Generate Auth Code** button and enter a domain name.
2. The system requests a new code from the registry.
3. Once generated, use **Reveal** to view or **Copy** to clipboard.

## Important Notes

- Auth codes are required for both incoming and outgoing transfers.
- Some registries auto-expire auth codes after a set period.
- Not all TLDs use EPP auth codes. Check [Registry Rules](registry-rules.md) for TLD-specific transfer mechanisms.

## Related Pages

- [Incoming Transfers](incoming-transfers.md)
- [Outgoing Transfers](outgoing-transfers.md)
- [Registry Rules](registry-rules.md)

[Back to Reseller Manual index](../index.md)
