# Register Domain Flow (Regiser Domains)

This document describes the UI and backend flow for selling a domain using the "Regiser Domains" integration. It uses the current API implemented in the web client (`/api/v1/RegisteredDomains/*`).

## Purpose
Define the step-by-step registration flow (search, suggest, configure, add to order) and map each step to the current API calls and expected payloads.

## Actors
- Customer/Admin/Sales user (UI)
- Web client (browser)
- Platform backend (RegisteredDomains endpoints)
- Register Domains provider (external registrar)

## Preconditions
- User is authenticated.
- Customer record exists (customer number can be entered).
- Registrar integrations configured; backend exposes registrar list with an `IsDefault` flag used by the UI to set initial focus.

## Relevant API endpoints (current)
- `POST /api/v1/RegisteredDomains/check-availability` � body: `{ domainName }` � returns `{ isAvailable, domainName, message, suggestedAlternatives? }`.
- `GET /api/v1/RegisteredDomains/pricing/{tld}?registrarId={id}` � returns pricing metadata for a TLD.
- `GET /api/v1/RegisteredDomains/available-tlds` � returns list of TLDs and pricing.
- `POST /api/v1/RegisteredDomains/register` � register domain for current authenticated user (self-service).
- `POST /api/v1/RegisteredDomains/register-for-customer` � register on behalf of a specific customer (admin/sales). Body includes `customerId` and `registrarId`.
- Registrar list endpoint (UI should use platform's registrars endpoint) � list items include `IsDefault`.

## Registration flow (step-by-step)

1. Enter customer number.
   - UI: allow entering a numeric customer number. If registering as self-service, this may be optional.

2. Select registrar from combo. Set default focus using `IsDefault`.
   - UI: fetch registrar list and pick the registrar with `IsDefault === true` as the initial selection.

3. Enter domain name (full name, e.g., `example.com`).

4. Check if TLD is supported by platform/selected registrar.
   - UI: extract TLD (portion after last `.`) and call `GET /api/v1/RegisteredDomains/pricing/{tld}?registrarId={id}` or use the cached `available-tlds` list.
   - If pricing returns null / 404, the TLD is not supported for that registrar.

5. Check if domain is available.
   - API call: `POST /api/v1/RegisteredDomains/check-availability` with `{ domainName }`.
   - Response example (available): `{ isAvailable: true, domainName: 'example.com', message: 'Available' }`.
   - Response example (not available): `{ isAvailable: false, domainName: 'example.com', message: 'Taken', suggestedAlternatives: ['example.net', 'example.co'] }`.

6. If domain is available, continue to step 20.

10. If domain is not available, create a list with suggested domain names using `suggestedAlternatives` from the availability response.

11. If user selects one of the suggested domains, set that selected domain into the domain input and go to step 20. If user does not select, go back to step 3 to enter a new domain name.

20. Enter registration properties (years, auto-renew, privacy protection, notes).
   - UI should present pricing and estimated total using pricing data fetched earlier: `total = registrationPrice * years + (privacy ? privacyPrice * years : 0)`.

21. Add to order.
   - For self-service (no `customerId`): call `POST /api/v1/RegisteredDomains/register` with body:
     `{ domainName, years, autoRenew, privacyProtection, notes }`.
   - For admin/sales registering for a customer: call `POST /api/v1/RegisteredDomains/register-for-customer` with body:
     `{ customerId, domainName, registrarId, years, autoRenew, privacyProtection, notes }`.
   - Backend will create an order/invoice and return a result containing `success`, `orderId` (or `orderNumber`), `totalAmount`, and potentially `requiresApproval` or status information.

## Notes and UI behavior
- Use inline validation for domain syntax prior to availability calls.
- When checking TLD support, prefer `GET /available-tlds` once and cache results for the UI session to reduce API calls.
- Show spinners and disable submit buttons during network operations.
- If registration returns `requiresApproval` or `Provisioning` states, surface those messages on the UI and provide an order number so users can track status.
- On failure (payment failure, registrar rejection, network error), present clear actionable messages and allow retry or alternative suggestions.

## Example payloads
- Check availability: `POST /api/v1/RegisteredDomains/check-availability` � body: `{ "domainName": "example.com" }`.
- Register for customer: `POST /api/v1/RegisteredDomains/register-for-customer` � body:
  `{ "customerId": 123, "domainName": "example.com", "registrarId": 45, "years": 1, "autoRenew": true, "privacyProtection": false, "notes": null }`.

## Edge cases
- TLD-specific registration requirements: some TLDs require extra registrant data; if backend returns validation errors, show the required fields and do not submit until provided.
- Simultaneous purchase: backend must perform final availability and hold the domain during order/invoice creation to prevent race conditions.

---

This document maps the high-level registration steps to the platform's current APIs so the UI and backend teams can implement the Register Domain flow consistently with the existing endpoints.
