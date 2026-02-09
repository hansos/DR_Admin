# Order to Invoice Flow

This document describes how a programmer can use the Billing & Sales API to create an order, process payment, and send the invoice.

## Authentication

All requests require a JWT in the `Authorization` header. Use the authentication endpoints in your project to obtain a token before calling the APIs.

## High-level steps

1. Ensure the customer exists
   - Use `GET /api/v1/Customers` or `POST /api/v1/Customers` to look up or create the customer.
   - Save the `customerId` for the order payload.

2. Gather pricing and billing data
   - Retrieve currency with `GET /api/v1/Currencies`.
   - Retrieve billing cycles with `GET /api/v1/BillingCycles` if the order will create a subscription.
   - Determine products, quantities, unit prices, and applicable tax rules (`GET /api/v1/TaxRules`).
   - Optionally validate coupon codes via `GET/POST /api/v1/Coupons`.

3. Create the order (draft)
   - Call `POST /api/v1/Orders` with a body containing `customerId`, `currency`, `items` (product id, quantity, unit price), optional `billingCycleId`, and optional `couponCode`.
   - The server should return an `orderId`, computed totals, taxes, and a draft invoice or quote.

4. Prepare payment
   - Select a payment gateway via `GET /api/v1/PaymentGateways` if applicable.
   - Create a payment intent with `POST /api/v1/PaymentIntents` including `orderId`, `amount`, `currency`, and a `paymentMethodToken` or client-side token reference.
   - Payment intent response may include a `clientSecret`, redirect URL, or other client-side data required to complete payment.

5. Complete payment on the client
   - Use the gateway's client SDK or redirect flow to collect card details and complete any required authentication (3DS, etc.).
   - The gateway will confirm the payment and update the payment intent status (or return a success callback to your server).

6. Confirm payment and finalize invoice
   - Poll or receive webhooks for the payment intent status. On success, mark the payment as captured/paid via your API if required.
   - Finalize or create the invoice for the order: `POST /api/v1/Invoices` or an order finalization action (implementation-specific).

7. Send the invoice
   - If the API exposes an explicit send endpoint, call `POST /api/v1/Invoices/{id}/send`.
   - Otherwise update the invoice `status` to `sent` using `PUT /api/v1/Invoices/{id}` to trigger email/notification sending.

8. Post-payment bookkeeping
   - Record payment and invoice references on the `Customer` and `Order` resources.
   - If the order created a subscription, create `POST /api/v1/Subscriptions` and track recurring billing entries (`GET /api/v1/SubscriptionBillingHistories`).

9. Error handling and idempotency
   - Handle `4xx` validation responses and display errors to the client.
   - Use idempotency keys for `POST` requests that may be retried (orders, payment intents) to avoid duplicates.
   - Keep sensitive card operations on the client side or use tokenization provided by the payment gateway.

## Notes and best practices

- Always include the JWT in the `Authorization` header for API calls.
- Validate totals server-side before capturing payment to avoid mismatches.
- Use webhooks from the payment gateway to handle asynchronous status changes (failed payments, chargebacks, dispute events).
- For recurring subscriptions rely on `BillingCycles` and scheduled jobs or webhook-driven invoice creation.

Refer to the individual controller documentation in this folder for exact request/response shapes and status codes.
