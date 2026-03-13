# Tax Evidence

Review collected evidence (IP address, billing address, etc.) used to determine customer tax jurisdiction.

## How to Access

Navigate to **Billing & Finance > Tax Evidence** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Customer | The customer this evidence relates to. |
| IP Country | Country determined from the customer's IP address. |
| Billing Country | Country from the customer's billing address. |
| Resolved Jurisdiction | The jurisdiction the system determined. |
| Confidence | Match confidence (e.g., High if IP and billing address agree). |
| Date | When the evidence was last collected. |
| Actions | View Details, Override. |

## How Evidence Is Used

The system collects multiple data points to determine a customer's tax jurisdiction for EU VAT and similar regulations:
1. IP address geolocation.
2. Billing address on file.
3. Any additional signals (bank country, phone prefix).

If signals conflict, the **Override** action allows manual resolution.

## Related Pages

- [Tax Jurisdictions](tax-jurisdictions.md)
- [Tax Rules](tax-rules.md)
- [Customer List](../customers/customer-list.md)

[Back to Reseller Manual index](../index.md)
