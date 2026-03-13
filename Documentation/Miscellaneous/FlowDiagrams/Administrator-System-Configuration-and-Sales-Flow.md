## Administrator configuration flow (catalog, pricing, payment routing)

### A) Global settings

```mermaid
flowchart TD
    A1[Admin signs in]
    A2[Open system settings]
    A3[GET /api/v1/SystemSettings]
    A4[Create setting]
    A5[POST /api/v1/SystemSettings]
    A6[Update setting]
    A7[PUT /api/v1/SystemSettings/:id]

    A1 --> A2 --> A3 --> A4 --> A5 --> A6 --> A7
```

### B) Payment gateways and instruments

```mermaid
flowchart TD
    B1[Admin opens payment configuration]
    B2[GET /api/v1/PaymentGateways]
    B3[POST /api/v1/PaymentGateways]
    B4[PUT /api/v1/PaymentGateways/:id]
    B5[POST /api/v1/PaymentGateways/:id/set-default]
    B6[POST /api/v1/PaymentGateways/:id/set-active]
    B7[GET /api/v1/PaymentInstruments]
    B8[POST /api/v1/PaymentInstruments]
    B9[PUT /api/v1/PaymentInstruments/:id]
    B10[Set instrument DefaultGatewayId]
    B11[Runtime: payment uses selected instrument]
    B12[API resolves instrument.DefaultGatewayId]
    B13[Gateway selected for processing]

    B1 --> B2 --> B3 --> B4 --> B5 --> B6 --> B7 --> B8 --> B9 --> B10 --> B11 --> B12 --> B13
```

### C) Registrar and TLD coverage

```mermaid
flowchart TD
    C1[Open registrar coverage]
    C2[Download registrar TLD list]
    C3[POST /api/v1/Registrars/:registrarId/tlds/download]
    C4[Assign specific TLD]
    C5[POST /api/v1/Registrars/:registrarId/tld/:tldId]
    C6[Review mapped offerings]
    C7[GET /api/v1/RegistrarTlds]

    C1 --> C2 --> C3 --> C4 --> C5 --> C6 --> C7
```

### D) Sales pricing setup

```mermaid
flowchart TD
    D1[Open pricing management]
    D2[Check current price]
    D3[GET /api/v1/tld-pricing/sales/tld/:tldId/current]
    D4[Create new price]
    D5[POST /api/v1/tld-pricing/sales]
    D6[Adjust future price]
    D7[PUT /api/v1/tld-pricing/sales/:id]
    D8[Archive old data]
    D9[POST /api/v1/tld-pricing/archive]

    D1 --> D2 --> D3 --> D4 --> D5 --> D6 --> D7 --> D8 --> D9
```
