# Webhooks

Configure webhook endpoints to receive real-time event notifications from the system.

## How to Access

Navigate to **Integrations > Webhooks** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Endpoint Name | Friendly name for the webhook. |
| URL | The endpoint URL that receives events. |
| Events | Which events trigger this webhook. |
| Status | Active, Disabled, Failing. |
| Last Triggered | Date of the last event delivery. |
| Actions | Edit, Test, View Log, Disable, Delete. |

## Adding a Webhook

1. Click **Add Webhook**.
2. Enter a name and the target URL.
3. Select the events to subscribe to:
   - Domain Registered, Renewed, Expired, Transferred.
   - Order Created, Completed, Failed.
   - Payment Received, Refunded.
   - Invoice Created, Sent, Paid.
4. Optionally add a secret key for request signature verification.
5. Click **Save**.

## Testing

Click **Test** to send a sample event payload to your endpoint and verify it responds with HTTP 200.

## Retry Policy

Failed deliveries are retried automatically with exponential backoff. After repeated failures, the webhook status changes to **Failing**.

## Related Pages

- [Automated Actions](../alerts/automated-actions.md)

[Back to Reseller Manual index](../index.md)
