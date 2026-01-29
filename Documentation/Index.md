# DR_Admin Features Overview


**DR_Admin** is a comprehensive REST API backend designed to simplify and automate essential web services for resellers, developers, and IT administrators. Its core capabilities span three main areas: domain registration, hosting management, and email delivery.  

Being a REST API, **DR_Admin can be accessed programmatically by any client written in any language**, making it highly flexible for integration into web applications, desktop tools, or automation scripts. Each component is built with flexibility, reliability, and integration in mind, supporting multiple providers and advanced features.

---

## API Documentation

- For installation hints , see the [Installation section](Installation/Installation.md).
- For detailed API reference documentation, see the [API Documentation](API/index.md).

---

## Feature Documentation

### 1. Domain Registration

The **Domain Registration module** enables programmatic management of domain names across a wide range of registrars. Through the `IDomainRegistrar` interface, applications can check domain availability, register, renew, and transfer domains, as well as manage DNS and domain settings.  

**Key Features:**

- **Registrar Support:** Integration with major registrars such as Namecheap, GoDaddy, Cloudflare, AWS Route 53, OpenSRS, and others. Each provider includes required authentication and configuration options.
- **Domain Lifecycle Management:**  
  - **Check Availability:** Quickly verify if a domain is available for registration.  
  - **Registration & Renewal:** Create new domains and extend existing registrations.  
  - **Transfer Domains:** Seamlessly move domains between registrars.  
  - **Update Nameservers & Privacy Settings:** Maintain control over DNS delegation and WHOIS privacy.  
  - **Auto-Renewal:** Optional automation to ensure uninterrupted domain ownership.
- **DNS Management:** Add, update, or remove DNS records programmatically. Full support for zone updates ensures that domains remain correctly configured for web and mail services.

See [Domain registration](Features/Domain-registration.md) for more information.

---

### 2. Hosting Management

The **Hosting module** (detailed in DR_Admin documentation) provides tools for managing web hosting accounts, services, and server resources. It is designed to allow administrators to provision, update, and monitor hosting services efficiently.  

**Typical Hosting Features:**

- Create and manage web hosting accounts and packages.  
- Configure server resources such as storage, bandwidth, and databases.  
- Automate account creation and suspension workflows.  
- Monitor server status, usage metrics, and alerts.  
- Support for reseller operations, enabling control over multiple end-users.

See [Hosting](Features/Hosting.md) for more information.


---

### 3. Email Delivery

The **Mailing module** allows applications to send emails asynchronously with support for plain text, HTML, and attachments, through the `IEmailSender` interface. DR_Admin supports multiple providers to give maximum flexibility for different email environments.

**Key Features:**

- **Provider Support:** SMTP, MailKit, Microsoft Graph API, Exchange, SendGrid, Mailgun, Amazon SES, Postmark, and others. Each provider has configurable settings like credentials, host, port, and security options.
- **Sending Emails:**  
  - **Basic Emails:** Send simple text or HTML emails to one or multiple recipients.  
  - **Attachments:** Include one or more files with outgoing emails.  
- **Asynchronous Operations:** All methods are fully async for integration with modern applications, improving performance and responsiveness.  
- **Error Handling:** Built-in exception support for invalid addresses, network failures, or authentication issues.


See [Mailing](Features/Mailing.md) for more information.

---

### 4. Full Database Backend

DR_Admin includes a **complete database server backend** to manage customers, services, orders, and invoicing. This allows the system to maintain all necessary business data in a structured, reliable manner.  

**Key Features:**

- **Customer Management:** Store and manage customer information, including contact details and account status.  
- **Service Management:** Track hosting, domain, and email services assigned to customers.  
- **Order Management:** Handle service orders, activations, and status updates.  
- **Invoicing & Billing:** Generate invoices, track payments, and manage financial records.  
- **Database Flexibility:** The backend can be powered by multiple database engines, including:
  - Microsoft SQL Server (MS SQL)  
  - SQLite  
  - MySQL  
  - MariaDB  
  - PostgreSQL  

This flexibility allows DR_Admin to be deployed in small environments with lightweight databases (like SQLite) or in large-scale production systems using robust servers like MS SQL or PostgreSQL.

---

### 5. Cross-Platform Compatibility

DR_Admin runs on **all operating systems that support .NET**, giving you the flexibility to deploy in diverse environments. Supported operating systems include:

- **Windows** (Windows 10, 11, Server 2016/2019/2022)  
- **Linux** (Ubuntu, Debian, CentOS, Red Hat Enterprise Linux, Fedora, openSUSE, and others)  
- **macOS** (Big Sur, Monterey, Ventura, and later versions)  

This ensures that DR_Admin can be hosted in any environment, whether on-premises servers, cloud infrastructure, or development machines.


---
### Summary

DR_Admin is a unified platform designed to manage the critical aspects of online services: domain registration, hosting management, and email delivery. Its modular approach, support for multiple providers, and asynchronous operations make it an ideal solution for resellers, developers, and IT administrators who require automation, flexibility, and reliability in managing web services.

See 
- [Domain registration](Features/Domain-registration.md)
- [Hosting](Features/Hosting.md)
- [Mailing](Features/Mailing.md)
