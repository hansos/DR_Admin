# Reseller Panel User Manual

## Dashboard

- [Summary](dashboard/summary.md) — Overview of your reseller account with key metrics, recent activity, and quick-access widgets.
- [New Quote](dashboard/new-quote.md) — Create a new domain registration, transfer, or renewal quote for a customer.
- [Expiring Domains](dashboard/expiring-domains.md) — View domains approaching their expiration date and take renewal action.
- [Unpaid Invoices](dashboard/unpaid-invoices.md) — List of outstanding invoices requiring payment or follow-up.
- [Ongoing Transfers](dashboard/ongoing-transfers.md) — Monitor domain transfers currently in progress.
- [Alerts](dashboard/alerts.md) — View important notifications and system alerts requiring attention.
- [Quick Actions](dashboard/quick-actions.md) — Shortcuts to frequently used operations like domain lookups and renewals.
- [Change Password](dashboard/change-password.md) — Update your account password.

## Domains

- [Domain List](domains/domain-list.md) — Browse, search, and filter all domains in your portfolio.
- [Domain Details](domains/domain-details.md) — View and manage detailed information for a specific domain including contacts, nameservers, and status.
- [Registrants](domains/registrants.md) — Manage domain registrant (owner) contact information and handle registrant changes.
- [Bulk Operations](domains/bulk-operations.md) — Perform actions on multiple domains at once, such as bulk renewals, DNS changes, or registrant updates.

## DNS & Nameservers

- [DNS Zones](dns/dns-zones.md) — Create and manage DNS zone records (A, AAAA, CNAME, MX, TXT, etc.) for your domains.
- [Nameserver Management](dns/nameserver-management.md) — Configure and manage nameservers assigned to your domains.
- [DNS Templates](dns/dns-templates.md) — Create reusable DNS record templates for quick zone setup on new domains.
- [Change History](dns/change-history.md) — Review a log of all DNS and nameserver changes with timestamps and user attribution.
- [Troubleshooting](dns/troubleshooting.md) — Diagnose and resolve common DNS resolution issues using built-in lookup tools.

## Customers

- [Customer List](customers/customer-list.md) — Browse, search, and manage all customer accounts in your reseller portfolio.
- [Contact Persons](customers/contact-persons.md) — Manage contact persons associated with customer accounts.
- [Communication](customers/communication.md) — View and send communications to customers, including emails and internal notes.

## Transfers

- [Incoming Transfers](transfers/incoming-transfers.md) — Manage domains being transferred into your reseller account from other registrars.
- [Outgoing Transfers](transfers/outgoing-transfers.md) — Manage domains being transferred out of your reseller account to other registrars.
- [Auth Codes](transfers/auth-codes.md) — Generate and manage authorization (EPP) codes required for domain transfers.
- [Status & Errors](transfers/status-and-errors.md) — Monitor transfer status and troubleshoot transfer failures with detailed error information.
- [Registry Rules](transfers/registry-rules.md) — View transfer policies, lock periods, and requirements per domain registry.

## Billing & Finance

- [Invoices](billing/invoices.md) — View, create, and manage customer invoices for domain services.
- [Orders](billing/orders.md) — Track and manage domain registration, renewal, and transfer orders.
- [Payments](billing/payments.md) — Record and view payments received from customers.
- [Billing Cycles](billing/billing-cycles.md) — Configure recurring billing periods for domain services and subscriptions.
- [Payment Gateways](billing/payment-gateways.md) — Configure payment processing gateways for accepting online payments.
- [Payment Instruments](billing/payment-instruments.md) — Define available payment methods customers can select (e.g., credit card, PayPal, bank transfer). The API resolves the default gateway configured for the selected instrument.
- [Currencies](billing/currencies.md) — Manage supported currencies and exchange rates for multi-currency billing.
- [Coupons](billing/coupons.md) — Create and manage discount coupons and promotional codes.
- [Tax Jurisdictions](billing/tax-jurisdictions.md) — Define geographic tax regions and their applicable tax rates.
- [Tax Registrations](billing/tax-registrations.md) — Manage your company's tax registration numbers (e.g., VAT ID) per jurisdiction.
- [Tax Categories](billing/tax-categories.md) — Classify products and services into tax categories for correct tax treatment.
- [Tax Rules](billing/tax-rules.md) — Configure rules that determine which tax rates apply to specific transactions based on jurisdiction, category, and customer type.
- [Order Tax Snapshots](billing/order-tax-snapshots.md) — View the tax calculations captured at the time of order placement for audit purposes.
- [Tax Evidence](billing/tax-evidence.md) — Review collected evidence (IP, billing address, etc.) used to determine customer tax jurisdiction.
- [Tax Quote Tool](billing/tax-quote-tool.md) — Preview tax calculations for a potential order before finalizing.
- [Tax Finalize Tool](billing/tax-finalize-tool.md) — Finalize and lock tax calculations for completed orders.
- [Invoice Tax Audit](billing/invoice-tax-audit.md) — Audit invoices to verify correct tax application and identify discrepancies.
- [Pricing](billing/pricing.md) — Set and manage pricing for products and services across your reseller account.
- [Profit Margins](billing/profit-margins.md) — Analyze profit margins across products, TLDs, and customers.
- [Reports](billing/reports.md) — Generate billing and financial summary reports.

## TLD & Registry

- [TLD List](tld/tld-list.md) — Browse all available top-level domains and their current status and availability.
- [Registry Rules](tld/registry-rules.md) — View registration rules, eligibility requirements, and policies per TLD registry.
- [Pricing per TLD](tld/pricing-per-tld.md) — View and configure registration, renewal, and transfer pricing for each top-level domain.
- [Campaigns & Discounts](tld/campaigns-and-discounts.md) — Manage promotional campaigns, seasonal offers, and TLD-specific discounts.

## Alerts & Automation

- [Notifications](alerts/notifications.md) — View and configure system notifications and alert preferences.
- [Automated Actions](alerts/automated-actions.md) — Set up automated workflows triggered by events such as auto-renew, expiration reminders, and follow-ups.
- [Templates](alerts/templates.md) — Create and manage notification and email templates used by alerts and automation.

## Reports & Export

- [Domain Reports](reports/domain-reports.md) — Generate reports on domain portfolio metrics, registration trends, and expiration forecasts.
- [Customer Reports](reports/customer-reports.md) — Generate reports on customer activity, account status, and growth.
- [Finance Reports](reports/finance-reports.md) — Generate financial reports including revenue, costs, margins, and tax summaries.
- [Export](reports/export.md) — Export data in various formats (CSV, Excel, PDF) for external use.

## Integrations

- [Registry APIs](integrations/registry-apis.md) — Configure connections to domain registry API providers (EPP/API credentials and endpoints).
- [Payment Solutions](integrations/payment-solutions.md) — Manage integrations with payment service providers for processing transactions.
- [Accounting](integrations/accounting.md) — Configure integrations with external accounting and ERP systems.
- [Email & SMS](integrations/email-and-sms.md) — Set up email and SMS delivery providers for sending notifications and alerts.
- [Webhooks](integrations/webhooks.md) — Configure webhook endpoints to receive real-time event notifications from the system.

## Infrastructure

- [Servers](infrastructure/servers.md) — Manage your server inventory, view status, and assign services.
- [IP Addresses](infrastructure/ip-addresses.md) — Manage and allocate IP addresses across servers and services.
- [Server Types](infrastructure/server-types.md) — Define server type classifications (e.g., web server, mail server, DNS server).
- [Operating Systems](infrastructure/operating-systems.md) — Manage the list of supported operating systems available for server provisioning.
- [Services](infrastructure/services.md) — Define and manage service types running on your infrastructure.
- [Hosting Plans](infrastructure/hosting-plans.md) — Configure hosting packages offered to customers with resource limits and features.
- [Hosting Providers](infrastructure/hosting-providers.md) — Manage hosting provider accounts and API credentials.
- [Hosting Panels](infrastructure/hosting-panels.md) — Define supported control panel types (e.g., cPanel, Plesk, DirectAdmin).
- [Panel Instances](infrastructure/panel-instances.md) — Manage individual control panel installations deployed on your servers.

## Users & Roles

- [User Management](users/user-management.md) — Create, edit, and manage user accounts for the reseller panel.
- [Roles & Permissions](users/roles-and-permissions.md) — Define roles and assign granular permissions to control user access across the panel.
- [Login History](users/login-history.md) — Review login activity and session history for all users.

## Settings

- [Company Setup](settings/company-setup.md) — Configure your company name, address, logo, and branding details.
- [Countries](settings/countries.md) — Manage the list of countries available throughout the system.
- [Default Values](settings/default-values.md) — Set system-wide default values for forms, operations, and new records.
- [Language & Region](settings/language-and-region.md) — Configure language, locale, date format, and regional formatting preferences.
- [Deadlines](settings/deadlines.md) — Set default deadlines for renewals, transfers, and other time-sensitive operations.
- [Security](settings/security.md) — Configure security settings including authentication policies, session timeouts, and access controls.

## Help & Support

- [Documentation](help/documentation.md) — Access the user manual and system documentation.
- [Registry Manuals](help/registry-manuals.md) — Access registry-specific documentation, policies, and technical guidelines.
- [Support Tickets](help/support-tickets.md) — Create and manage support requests with the DR Admin support team.
- [System Status](help/system-status.md) — View current system health, uptime, and service status.
- [Debug](help/debug.md) — Access diagnostic tools and debug information (visible to administrators only).
