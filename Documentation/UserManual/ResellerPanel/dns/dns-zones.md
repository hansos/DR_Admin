# DNS Zones

Create and manage DNS zone records (A, AAAA, CNAME, MX, TXT, etc.) for your domains.

## How to Access

Navigate to **DNS & Nameservers > DNS Zones** from the side menu.

## Page Layout

A list of all DNS zones, one per domain, with the ability to expand and view/edit records.

| Column | Description |
|--------|-------------|
| Domain | The domain this zone belongs to. |
| Records | Number of DNS records in the zone. |
| Last Modified | Date of the most recent change. |
| Actions | View/Edit Records, Apply Template, Delete Zone. |

## Managing Records

1. Click a domain to expand its zone.
2. Click **Add Record** to create a new record.
3. Select the record type and fill in the fields:

| Record Type | Fields |
|-------------|--------|
| A | Host, IPv4 Address, TTL |
| AAAA | Host, IPv6 Address, TTL |
| CNAME | Host, Target, TTL |
| MX | Host, Mail Server, Priority, TTL |
| TXT | Host, Value, TTL |
| SRV | Service, Protocol, Priority, Weight, Port, Target, TTL |
| NS | Host, Nameserver, TTL |

4. Click **Save** to apply the record.

## Applying a Template

1. Select a domain zone.
2. Click **Apply Template**.
3. Choose from available [DNS Templates](dns-templates.md).
4. Confirm to overwrite or merge with existing records.

## Related Pages

- [Nameserver Management](nameserver-management.md)
- [DNS Templates](dns-templates.md)
- [Change History](change-history.md)

[Back to Reseller Manual index](../index.md)
