# Tax Rules

Configure rules that determine which tax rates apply to specific transactions based on jurisdiction, category, and customer type.

## How to Access

Navigate to **Billing & Finance > Tax Rules** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Rule Name | Descriptive name for the rule. |
| Jurisdiction | The [Tax Jurisdiction](tax-jurisdictions.md) this rule applies to. |
| Category | The [Tax Category](tax-categories.md) this rule covers. |
| Customer Type | Business, Individual, or All. |
| Rate | Tax rate percentage applied by this rule. |
| Priority | Determines which rule takes precedence when multiple match. |
| Actions | Edit, Delete. |

## Adding a Rule

1. Click **Add Rule**.
2. Select the jurisdiction, category, and customer type.
3. Enter the tax rate.
4. Set the priority (lower number = higher priority).
5. Click **Save**.

## Rule Evaluation

When calculating tax on an order, the system:
1. Determines the customer's jurisdiction (from [Tax Evidence](tax-evidence.md)).
2. Matches the product's [Tax Category](tax-categories.md).
3. Finds the highest-priority matching rule.
4. Applies the configured rate.

## Related Pages

- [Tax Jurisdictions](tax-jurisdictions.md)
- [Tax Categories](tax-categories.md)
- [Tax Evidence](tax-evidence.md)
- [Tax Quote Tool](tax-quote-tool.md)

[Back to Reseller Manual index](../index.md)
