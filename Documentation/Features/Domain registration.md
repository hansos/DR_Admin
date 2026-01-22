# DomainRegistrationLib `IDomainRegistrar` Interface - User Manual

The `IDomainRegistrar` interface provides asynchronous methods for managing domain registrations, DNS records, and domain settings. It is designed for integration with domain registration systems and DNS management services.

---

## Table of Contents

1. [Domain Availability](#domain-availability)
    - Check Availability
2. [Domain Management](#domain-management)
    - Register Domain
    - Renew Domain
    - Transfer Domain
    - Get Domain Info
    - Update Nameservers
    - Privacy Protection
    - Auto-Renewal
3. [DNS Management](#dns-management)
    - Get DNS Zone
    - Update DNS Zone
    - Add DNS Record
    - Update DNS Record
    - Delete DNS Record

---

## Domain Availability

### `CheckAvailabilityAsync(string domainName)`
Checks if a domain is available for registration.

- **Parameters:**
  - `domainName` (`string`): The domain name to check (e.g., `example.com`).
- **Returns:** `Task<DomainAvailabilityResult>` — Indicates whether the domain is available and any related status messages.

---

## Domain Management

### `RegisterDomainAsync(DomainRegistrationRequest request)`
Registers a new domain.

- **Parameters:**
  - `request` (`DomainRegistrationRequest`): Details for registration (domain name, registrant info, registration period, etc.).
- **Returns:** `Task<DomainRegistrationResult>` — Contains registration status and domain details.

---

### `RenewDomainAsync(DomainRenewalRequest request)`
Renews an existing domain.

- **Parameters:**
  - `request` (`DomainRenewalRequest`): Details for renewal (domain name, renewal period).
- **Returns:** `Task<DomainRenewalResult>` — Renewal status and expiry information.

---

### `TransferDomainAsync(DomainTransferRequest request)`
Transfers a domain from another registrar.

- **Parameters:**
  - `request` (`DomainTransferRequest`): Transfer details (domain name, auth code, registrant info).
- **Returns:** `Task<DomainTransferResult>` — Transfer status and messages.

---

### `GetDomainInfoAsync(string domainName)`
Gets detailed information about a domain.

- **Parameters:**
  - `domainName` (`string`): Domain name.
- **Returns:** `Task<DomainInfoResult>` — Includes registration details, expiry date, and registrar information.

---

### `UpdateNameserversAsync(string domainName, List<string> nameservers)`
Updates the nameservers for a domain.

- **Parameters:**
  - `domainName` (`string`): Domain name.
  - `nameservers` (`List<string>`): List of nameservers to assign (e.g., `n
