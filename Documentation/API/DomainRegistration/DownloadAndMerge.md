
# DownloadDomainsForRegistrarAsync Endpoint

This document describes the `DownloadDomainsForRegistrarAsync` endpoint, including its purpose, required tables, prerequisites, common failure scenarios, and setup checklist.

---

## Endpoint Details

- **HTTP Method:** `POST`  
- **URL:** `/api/registrars/{registrarId}/domains/download`  
- **Controller Method:** `DownloadDomainsForRegistrar(int)` in `RegistrarsController`

---

## Purpose

The `DownloadDomainsForRegistrarAsync` endpoint fetches domain information from an external registrar (e.g., NameCheap, GoDaddy) and merges it into the local database. Specifically, it:

1. Fetches domains from the external registrar API.
2. Merges domains into the database:
   - Creates or updates **Domain** records.
   - Creates or updates **TLD** records.
   - Creates or updates **RegistrarTld** relationships.
   - Merges **DomainContact** information for each domain.
3. Returns the total count of successfully saved domains (created + updated).

---

## Required Tables & Prerequisites

For this endpoint to succeed, certain tables and data must exist or be initialized:

### 1. Registrars ✅ Required
- Must have an active registrar record with:
  - `IsActive = true`
  - A valid `Code` (e.g., `"NAMECHEAP"`, `"GODADDY"`)
  - Proper API credentials configured

### 2. Customers ✅ Critical
- Customer records must exist with emails matching the domain registrant emails.
- Lookup sequence for identifying a customer:
  1. `Registrant` email
  2. `Admin` email
  3. First available email
- Domains are **skipped** if no matching customer is found.

### 3. ServiceTypes ✅ Required
- Must contain a service type with `Name = "DOMAIN"`.
- Used when creating the service for each domain.
- Missing this record will result in the error:
```
Service type 'DOMAIN' not found in database
```


### 4. ResellerCompanies ✅ Required
- At least one reseller company must be marked as default.
- Used when creating services for domains.
- Accessed via `_resellerCompanyService.GetDefaultResellerCompanyAsync()`.

### 5. Tlds ⚠️ Optional (Auto-created)
- TLD records are automatically created if they do not exist.
- Extracted from domain names (e.g., `"com"` from `"example.com"`).

### 6. RegistrarTlds ⚠️ Optional (Auto-created)
- RegistrarTld relationships are automatically created if missing.
- Links registrar to TLD along with pricing information.

### 7. Services 📝 Created Automatically
- Service records are automatically created for each new domain.

### 8. Domains 📝 Created/Updated Automatically
- Domain records are created or updated during the merge process.

### 9. DomainContacts 📝 Created/Updated Automatically
- Contact records are merged for each domain.

---

## Common Failure Scenarios

When errors occur, they usually indicate missing data or misconfigurations:

### Customer-related Errors
- `"No customer found with email {email} for domain {domain}"` — Missing customer records
- `"Domain {domain} skipped: no customer could be identified"` — No valid contact email
- `"No contacts available for domain {domain}, cannot identify customer"` — Domain has no contact info

### Service-related Errors
- `"Service type 'DOMAIN' not found in database"` — Missing service type record

### Domain-related Errors
- `"Domain {domain} missing required dates"` — Missing `RegistrationDate` or `ExpirationDate`

**Example log:**  









