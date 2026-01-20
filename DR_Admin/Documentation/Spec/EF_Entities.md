# EF Entity Classes for ISP API Server

## 1. Customer & User Management

### Customer
- **Id** (PK)
- Name
- Email
- Phone
- Address
- CreatedAt
- UpdatedAt
- **Navigation Properties:** 
  - `Users` (1:N)
  - `Orders` (1:N)
  - `Domains` (1:N)
  - `HostingAccounts` (1:N)

### User
- **Id** (PK)
- CustomerId (FK, nullable for standalone admin)
- Username
- PasswordHash
- Email
- IsActive
- CreatedAt
- UpdatedAt
- **Navigation Properties:**
  - `Customer`
  - `Roles` (M:N via `UserRole`)
  - `Tokens` (1:N)
  - `AuditLogs` (1:N)

### Role
- **Id** (PK)
- Name (Admin, Customer, Support, etc.)
- Description
- **Navigation Properties:**
  - `Users` (M:N via `UserRole`)

### UserRole
- **UserId** (PK, FK)
- **RoleId** (PK, FK)

### Token
- **Id** (PK)
- UserId (FK)
- TokenType (Access, Refresh)
- TokenValue
- Expiry
- CreatedAt
- RevokedAt
- **Navigation Properties:** `User`

---

## 2. Product / Service Catalog

### Service
- **Id** (PK)
- Name
- Description
- ServiceTypeId (FK to `ServiceType`)
- BillingCycleId (FK to `BillingCycle`)
- Price
- CreatedAt
- UpdatedAt
- **Navigation Properties:** 
  - `Orders` (1:N)
  - `ServiceType`
  - `BillingCycle`

### ServiceType
- **Id** (PK)
- Name (Domain, Hosting, Email, Add-on)
- Description
- **Navigation Properties:** `Services` (1:N)

### BillingCycle
- **Id** (PK)
- Name (Monthly, Yearly, Quarterly, etc.)
- DurationInDays
- Description
- **Navigation Properties:** `Services` (1:N)

### Order / Subscription
- **Id** (PK)
- CustomerId (FK)
- ServiceId (FK)
- Status (Active, Pending, Cancelled)
- StartDate
- EndDate
- NextBillingDate
- **Navigation Properties:**
  - `Customer`
  - `Service`
  - `Invoices` (1:N)

---

## 3. Billing & Payments

### Invoice
- **Id** (PK)
- OrderId (FK)
- CustomerId (FK)
- Amount (total, sum of InvoiceLines)
- Status (Paid, Unpaid, Overdue)
- DueDate
- CreatedAt
- PaidAt
- **Navigation Properties:** 
  - `InvoiceLines` (1:N)
  - `PaymentTransactions` (1:N)

### InvoiceLine
- **Id** (PK)
- InvoiceId (FK)
- Description (e.g., "Domain registration example.com", "Hosting plan Pro")
- Quantity
- UnitPrice
- TotalPrice
- ServiceId (FK, optional)
- CreatedAt
- **Navigation Properties:** `Invoice`, `Service`

### PaymentTransaction
- **Id** (PK)
- InvoiceId (FK)
- PaymentMethod (Stripe, Card, etc.)
- Status
- TransactionId (from gateway)
- Amount
- CreatedAt
- **Navigation Properties:** `Invoice`

---

## 4. Domain Management

### Domain
- **Id** (PK)
- CustomerId (FK)
- Name
- ProviderId (FK)
- Status (Active, Pending, Expired)
- RegistrationDate
- ExpirationDate
- **Navigation Properties:** 
  - `DnsRecords` (1:N)
  - `Customer`

### DomainProvider
- **Id** (PK)
- Name (AWS, GoDaddy, etc.)
- ApiEndpoint
- ApiKey
- ApiSecret

### DnsRecord
- **Id** (PK)
- DomainId (FK)
- Type (A, CNAME, MX, TXT, etc.)
- Name
- Value
- TTL
- **Navigation Properties:** `Domain`

---

## 5. Hosting Integration

### HostingAccount
- **Id** (PK)
- CustomerId (FK)
- ServiceId (FK)
- Provider (cPanel, Plesk)
- Username
- PasswordHash
- Status
- CreatedAt
- ExpirationDate
- **Navigation Properties:** `Customer`, `Service`

---

## 6. Audit & Logging

### AuditLog
- **Id** (PK)
- UserId (FK)
- ActionType
- EntityType
- EntityId
- Timestamp
- Details
- IPAddress
- **Navigation Properties:** `User`

---

## 7. Configuration & Miscellaneous

### SystemSetting
- **Id** (PK)
- Key
- Value
- Description

### BackupSchedule
- **Id** (PK)
- DatabaseName
- Frequency
- LastBackupDate
- NextBackupDate
- Status

---

## Relationships Overview
- Customer → Users (1:N)  
- User → Roles (M:N via UserRole)  
- Customer → Orders (1:N)  
- Order → Service (N:1)  
- Order → Invoice (1:N)  
- Invoice → InvoiceLines (1:N)  
- Invoice → PaymentTransaction (1:N)  
- Customer → Domains (1:N)  
- Domain → DnsRecord (1:N)  
- Customer → HostingAccount (1:N)  
- InvoiceLine → Service (optional FK)  
- Service → ServiceType (N:1)  
- Service → BillingCycle (N:1)  
