# Troubleshooting

Diagnose and resolve common DNS resolution issues using built-in lookup tools.

## How to Access

Navigate to **DNS & Nameservers > Troubleshooting** from the side menu.

## Available Tools

### DNS Lookup

Query DNS records for a domain from multiple global resolvers. Enter a domain name and record type to see current live results.

### Propagation Check

Verify whether recent DNS changes have propagated across major DNS resolvers worldwide.

### WHOIS Lookup

Check the public WHOIS data for a domain, including registrar, nameservers, and expiry date.

## Common Issues

| Symptom | Possible Cause | Resolution |
|---------|---------------|------------|
| Domain not resolving | Missing A/AAAA record | Add the correct record in [DNS Zones](dns-zones.md). |
| Email not working | Missing or incorrect MX record | Verify MX records in [DNS Zones](dns-zones.md). |
| Changes not visible | DNS propagation delay | Wait up to 48 hours, or lower TTL values. |
| Wrong nameservers | Nameservers not updated at registry | Update via [Nameserver Management](nameserver-management.md). |

## Related Pages

- [DNS Zones](dns-zones.md)
- [Nameserver Management](nameserver-management.md)
- [Change History](change-history.md)

[Back to Reseller Manual index](../index.md)
