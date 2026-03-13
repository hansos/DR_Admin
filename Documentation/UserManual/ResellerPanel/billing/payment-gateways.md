# Payment Gateways

Configure payment processing gateways for accepting online payments.

## How to Access

Navigate to **Billing & Finance > Payment Gateways** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Gateway Name | Name of the payment gateway. |
| Provider | Gateway provider (e.g., Stripe, PayPal, Nets). |
| Status | Active or Disabled. |
| Default | Whether this is the default gateway for a payment instrument. |
| Actions | Edit, Enable/Disable, Test, Delete. |

## Adding a Gateway

1. Click **Add Gateway**.
2. Select the provider from the list.
3. Enter the API credentials (key, secret, merchant ID â€” varies by provider).
4. Configure supported currencies.
5. Set as default for one or more [Payment Instruments](payment-instruments.md).
6. Click **Save**.

## Testing

Use the **Test** action to send a test transaction through the gateway and verify connectivity.

## Related Pages

- [Payment Instruments](payment-instruments.md)
- [Payments](payments.md)
- [Payment Solutions (Integrations)](../integrations/payment-solutions.md)

[Back to Reseller Manual index](../index.md)
