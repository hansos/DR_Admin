# Invoice Tax Audit

Audit invoices to verify correct tax application and identify discrepancies.

## How to Access

Navigate to **Billing & Finance > Invoice Tax Audit** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Invoice # | The invoice being audited. |
| Customer | Invoiced customer. |
| Expected Tax | Tax amount calculated by current rules. |
| Invoiced Tax | Tax amount on the invoice. |
| Difference | Discrepancy between expected and invoiced tax. |
| Status | Match, Mismatch, or Pending Review. |
| Actions | View Details, Resolve. |

## Audit Process

1. Select a date range or set of invoices to audit.
2. The system recalculates tax using the [Order Tax Snapshot](order-tax-snapshots.md) and compares it to the invoiced amount.
3. Mismatches are flagged for review.
4. Use **Resolve** to document the reason for any discrepancy (e.g., rate change, manual override).

## Related Pages

- [Invoices](invoices.md)
- [Order Tax Snapshots](order-tax-snapshots.md)
- [Tax Rules](tax-rules.md)

[Back to Reseller Manual index](../index.md)
