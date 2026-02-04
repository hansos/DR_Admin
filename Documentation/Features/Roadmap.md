# DR_Admin Feature Roadmap

**DR_Admin** is a comprehensive REST API backend designed for resellers of domains, hosting, and email services.  
It provides programmatic access to domain registration, hosting management, and email delivery.  

> **Note:** This roadmap shows where we're going. It is not complete, and not all features are implemented yet.

---

## 1. Domain Management

### Features
- **Domain Search & Availability**
  - Check availability across multiple registrars.
  - Support for TLD-specific rules and pricing.
- **Domain Registration**
  - Register domains with multiple registrars.
  - Bulk registration support.
- **Domain Renewal & Expiration**
  - Manual and automatic renewals.
  - Notifications for upcoming expirations.
- **Domain Transfer**
  - Initiate and track inbound/outbound transfers.
- **Domain Ownership & Contact Management**
  - Update WHOIS/Registrant info.
  - Privacy protection / WHOIS proxy support.
- **DNS Management**
  - Add, update, or remove DNS records (A, AAAA, CNAME, MX, TXT, SRV, etc.).
  - Multiple name server support.
  - DNS zone import/export.
- **Domain Status & History**
  - Query domain status: active, pending, locked, suspended.
  - Retrieve registration date, expiration, registrar, and owner info.

### Workflows
1. **Register a domain**
   - Search availability → Select registrar/TLD → Register → Configure initial DNS
2. **Bulk renew domains**
   - Fetch expiring domains → Renew in batch
3. **Update DNS records**
   - Modify records via API → Propagate changes

---

## 2. Hosting Management

### Features
- **Account & Plan Management**
  - Create, update, suspend, delete accounts.
  - Assign hosting plans (storage, bandwidth, databases, emails, SSL).
- **Resource Management**
  - Track usage: storage, bandwidth, CPU, memory.
  - Upgrade or downgrade plans.
- **Website Deployment**
  - Upload/manage files.
  - Install CMS (WordPress, Joomla, Drupal).
- **Database Management**
  - Create/update/delete databases.
  - Manage users and privileges.
- **SSL/TLS Certificates**
  - Issue, renew, revoke (Let's Encrypt or commercial).
- **Server Monitoring**
  - Check server status and resources.
  - Alerts for errors or thresholds.

### Workflows
1. **Provision new hosting account**
   - Select plan → Create account → Set DNS → Deploy website/CMS
2. **Upgrade hosting plan**
   - Check resource usage → Change plan → Notify user
3. **Backup & restore**
   - Schedule backups → Restore files/databases

---

## 3. Email Delivery & Management

### Features
- **Mailbox Management**
  - Create, update, delete mailboxes.
  - Forwarding, aliases, storage quota.
- **Email Sending & Tracking**
  - Send transactional and marketing emails.
  - Track delivery, opens, bounces.
- **Spam & Security**
  - Configure SPF, DKIM, DMARC.
  - Blacklist/whitelist addresses.
- **Automation**
  - Auto-responders, vacation messages.
  - Mailing list management.

### Workflows
1. **Create mailbox**
   - Assign username/password → Set quota/forwarding
2. **Send email via API**
   - Input: sender, recipients, subject, content → Output: delivery status
3. **Configure domain email**
   - Update DNS for SPF, DKIM, DMARC → Validate deliverability

---

## 4. Reseller & User Management

### Features
- **Multi-Reseller Support**
  - Sub-accounts with limited permissions.
  - Track reseller usage/earnings.
- **User Authentication & Permissions**
  - API keys or OAuth tokens.
  - Role-based access (admin, reseller, client).
- **Billing & Invoicing**
  - Track usage for domains, hosting, email.
  - Generate invoices and integrate payment gateways.

### Workflows
1. **Create reseller account**
   - Assign quota and pricing → Generate API key
2. **Monitor client usage**
   - Retrieve reports → Apply billing rules

---

## 5. Reporting & Automation

### Features
- **Activity Logs**
  - Track all API actions for auditing.
- **Usage Reports**
  - Domains registered, hosting accounts active, emails sent.
- **Alerts & Notifications**
  - Expiration reminders
  - Resource limits
- **Bulk Operations**
  - Batch domain registration, renewal, DNS updates.
- **Scheduler / Cron Integration**
  - Automated renewals, backups, monitoring.

### Workflows
1. **Generate monthly usage report**
   - Fetch domain, hosting, email data → Summarize per reseller/client
2. **Set automated renewal**
   - Schedule API call → Send invoice/notification

---

> This roadmap outlines the future vision for **DR_Admin**. Features will be implemented incrementally as the project progresses.
