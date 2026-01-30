# DR_Admin

A comprehensive ISP/Domain Reseller administration system built with .NET 10. This solution provides a complete backend API and web interface for managing domain registrations, hosting services, billing, invoicing, and customer management.

---
⚠️  
DR_Admin is currently under active development.   
Several features are not yet implemented, and the system requires extensive testing.   
Functionality may change frequently, and the API is not considered stable.   
Use at your own discretion for development and testing purposes only.
---

[See developer's documentation](Documentation/Index.md)

## Solution Structure

| Project | Description |
|---------|-------------|
| **DR_Admin** | Core Web API with REST endpoints, Entity Framework Core database, and business logic |
| **DR_Admin_Web** | Blazor web frontend application |
| **DomainRegistrationLib** | Domain registrar integrations (Namecheap, GoDaddy, Cloudflare, AWS, and more) |
| **PaymentGatewayLib** | Payment gateway integrations (Stripe, PayPal, Braintree, Adyen, and 25+ providers) |
| **EmailSenderLib** | Email delivery integrations (SMTP, SendGrid, Mailgun, Amazon SES, Postmark, Microsoft Graph) |
| **SMSSenderLib** | SMS sender integrations (Twilio, Vonage, MessageBird, Africa's Talking, and more) |
| **HostingPanelLib** | Hosting control panel integrations (cPanel, Plesk, DirectAdmin, Virtualmin, CyberPanel) |
| **ExchangeRateLib** | Currency exchange rate provider integrations |
| **DR_Admin.IntegrationTests** | Integration test suite |

## Features

### Core Functionality
- **Customer Management** - Full customer lifecycle with contact persons, credit management, and payment methods
- **Order Processing** - Quote-to-order workflow with state machine-based lifecycle
- **Invoicing** - Invoice generation, payment tracking, and refund processing
- **Subscription Management** - Recurring billing with billing cycle support
- **Multi-Currency** - Currency exchange rate management with automatic updates

### Domain Services
- Domain availability checking
- Domain registration and renewal workflows
- Domain transfers
- DNS record management
- TLD and registrar configuration

### Hosting Services
- Hosting account provisioning
- Server and IP address management
- Control panel integration
- Hosting package management

### Communication
- Email queue with background processing
- SMS notifications
- Document templates for automated communications

### System Features
- JWT-based authentication with role-based authorization
- Configurable CORS policies
- Structured logging with Serilog
- Automatic database backups
- Domain expiration monitoring
- Outbox pattern for reliable event processing

## Database Support

- SQLite (default for development)
- SQL Server
- PostgreSQL

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Configuration

1. Clone the repository
2. Configure `appsettings.json` or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:\\Tmp\\DR_Admin.db"
  },
  "DbSettings": {
    "DatabaseType": "SQLITE"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForSecurity",
    "Issuer": "DR_Admin",
    "Audience": "DR_Admin_Users",
    "ExpirationInMinutes": 60
  }
}
```

### Running the Application

```bash
# Restore dependencies
dotnet restore

# Run the API
dotnet run --project DR_Admin

# Run the Web UI
dotnet run --project DR_Admin_Web
```

### API Documentation

Once running, Swagger UI is available at:
- `https://localhost:{port}/swagger`

## Integrations

### Domain Registrars
- Namecheap
- GoDaddy
- Cloudflare
- OpenSRS
- CentralNic
- DNSimple
- Domainbox
- Oxxa
- Regtons
- DomainNameAPI
- AWS Route 53

### Payment Gateways
- Stripe
- PayPal
- Braintree
- Adyen
- Square
- Authorize.Net
- Worldpay
- Checkout.com
- Mollie
- Klarna
- GoCardless
- Cybersource
- Paystack
- Flutterwave
- M-Pesa
- And more...

### Email Providers
- SMTP
- MailKit
- SendGrid
- Mailgun
- Amazon SES
- Postmark
- Microsoft Graph API
- Exchange

### SMS Providers
- Twilio
- Vonage
- MessageBird
- Sinch
- Africa's Talking
- Safaricom
- IntaSend
- GatewayAPI

### Hosting Panels
- cPanel
- Plesk
- DirectAdmin
- Virtualmin
- CyberPanel
- CloudPanel
- ISPConfig

## Testing

```bash
# Run integration tests
dotnet test DR_Admin.IntegrationTests
```

## License

This project is licensed under the MIT License.
See the LICENSE file for details.


## Contributing

Please read the contribution guidelines before submitting pull requests.