# Payment Instruments

Define available payment methods customers can select (e.g., credit card, PayPal, bank transfer). The API resolves the default gateway configured for the selected instrument.

## How to Access

Navigate to **Billing & Finance > Payment Instruments** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Instrument Name | Display name shown to customers (e.g., Credit Card, PayPal, Bank Transfer). |
| Type | Card, Wallet, Bank Transfer, Cash, Other. |
| Default Gateway | The [Payment Gateway](payment-gateways.md) used when this instrument is selected. |
| Status | Active or Disabled. |
| Actions | Edit, Enable/Disable, Delete. |

## How It Works

1. **Admin defines instruments** â€” Each instrument represents a payment method available to customers.
2. **Admin assigns a default gateway** â€” Each instrument is linked to a [Payment Gateway](payment-gateways.md) that processes payments for it.
3. **Customer selects an instrument** â€” When paying, the customer picks their preferred method.
4. **API resolves the gateway** â€” The system automatically routes the payment to the correct gateway based on the selected instrument.

## Adding an Instrument

1. Click **Add Instrument**.
2. Enter the display name and type.
3. Select the default [Payment Gateway](payment-gateways.md).
4. Click **Save**.

## Related Pages

- [Payment Gateways](payment-gateways.md)
- [Payments](payments.md)

[Back to Reseller Manual index](../index.md)
