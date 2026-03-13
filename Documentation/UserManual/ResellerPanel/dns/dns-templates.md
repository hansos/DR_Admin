# DNS Templates

Create reusable DNS record templates for quick zone setup on new domains.

## How to Access

Navigate to **DNS & Nameservers > DNS Templates** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Template Name | Descriptive name for the template. |
| Records | Number of DNS records in the template. |
| Last Modified | Date the template was last changed. |
| Actions | Edit, Duplicate, Delete, Apply to Domain. |

## Creating a Template

1. Click **Add Template**.
2. Enter a template name.
3. Add DNS records (same types as [DNS Zones](dns-zones.md): A, AAAA, CNAME, MX, TXT, etc.).
4. Use the placeholder `{domain}` in record values to auto-substitute the domain name when applied.
5. Click **Save**.

## Applying a Template

Templates can be applied from:
- This page â€” select a template and click **Apply to Domain**.
- The [DNS Zones](dns-zones.md) page â€” click **Apply Template** on a zone.
- [Bulk Operations](../domains/bulk-operations.md) â€” apply to multiple domains at once.

## Related Pages

- [DNS Zones](dns-zones.md)
- [Bulk Operations](../domains/bulk-operations.md)

[Back to Reseller Manual index](../index.md)
