# Nameserver Management

Configure and manage nameservers assigned to your domains.

## How to Access

Navigate to **DNS & Nameservers > Nameserver Management** from the side menu.

## Page Layout

A list of nameserver sets available in your account.

| Column | Description |
|--------|-------------|
| Name | Friendly name for the nameserver set. |
| Nameservers | List of NS hostnames (e.g., ns1.example.com, ns2.example.com). |
| Domains Using | Number of domains assigned to this set. |
| Actions | Edit, Delete, Assign to Domains. |

## Creating a Nameserver Set

1. Click **Add Nameserver Set**.
2. Enter a friendly name.
3. Add at least two nameserver hostnames.
4. Optionally add glue records (IP addresses) if the nameservers are under domains you manage.
5. Click **Save**.

## Assigning to Domains

1. Select a nameserver set.
2. Click **Assign to Domains**.
3. Choose one or more domains.
4. Confirm the update. Changes propagate to the registry.

## Related Pages

- [DNS Zones](dns-zones.md)
- [Domain Details](../domains/domain-details.md)
- [Change History](change-history.md)

[Back to Reseller Manual index](../index.md)
