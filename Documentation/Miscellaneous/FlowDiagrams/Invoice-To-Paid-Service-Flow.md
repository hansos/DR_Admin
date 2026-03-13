## Payment flow from invoice to paid service

```mermaid
flowchart TD
    A1[Invoice issued and waiting for payment]
    A2[GET /api/v1/Invoices/:invoiceId]
    A3[GET /api/v1/PaymentInstruments/active]
    A4[Customer selects instrument]
    A5[API resolves instrument DefaultGatewayId]
    A6[GET /api/v1/CustomerPaymentMethods/customer/:customerId/default]
    A7[POST /api/v1/Payments/process]
    A8{Payment requires additional authentication}
    A9[POST /api/v1/Payments/confirm-authentication/:paymentAttemptId]
    A10[POST /api/v1/Payments/webhook/:gatewayName]
    A11[GET /api/v1/Payments/attempts/invoice/:invoiceId]
    A12{Payment succeeded}
    A13[Invoice updated to Paid]
    A14[GET /api/v1/Orders/:orderId]
    A15[PUT /api/v1/Orders/:orderId]
    A16[Order status Active paid service]
    A17[GET /api/v1/Services/:serviceId]
    A18[Service delivered active to customer]
    A19[POST /api/v1/Payments/retry/:paymentAttemptId]

    A1 --> A2 --> A3 --> A4 --> A5 --> A6 --> A7 --> A8
    A8 -->|Yes| A9 --> A10 --> A11 --> A12
    A8 -->|No| A10 --> A11 --> A12
    A12 -->|Yes| A13 --> A14 --> A15 --> A16 --> A17 --> A18
    A12 -->|No| A19 --> A11
```

### Endpoints included

- `GET /api/v1/Invoices/:invoiceId`
- `GET /api/v1/PaymentInstruments/active`
- `GET /api/v1/CustomerPaymentMethods/customer/:customerId/default`
- `POST /api/v1/Payments/process`
- `POST /api/v1/Payments/confirm-authentication/:paymentAttemptId`
- `POST /api/v1/Payments/webhook/:gatewayName`
- `GET /api/v1/Payments/attempts/invoice/:invoiceId`
- `POST /api/v1/Payments/retry/:paymentAttemptId`
- `GET /api/v1/Orders/:orderId`
- `PUT /api/v1/Orders/:orderId`
- `GET /api/v1/Services/:serviceId`
