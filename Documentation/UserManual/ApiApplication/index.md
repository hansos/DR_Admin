# API Application Manual

## Address Types

Manages address type information including creation, retrieval, updates, and deletion

- [GET /api/v1/address-types](address-types/get-get-all-address-types-api-v1-address-types.md) — Manages address type information including creation, retrieval, updates, and deletion
- [GET /api/v1/address-types/{id}](address-types/get-get-address-type-by-id-api-v1-address-types-id.md) — Retrieves a specific address type by its unique identifier
- [POST /api/v1/address-types](address-types/post-create-address-type-api-v1-address-types.md) — Creates a new address type in the system
- [PUT /api/v1/address-types/{id}](address-types/put-update-address-type-api-v1-address-types-id.md) — Updates an existing address type
- [DELETE /api/v1/address-types/{id}](address-types/delete-delete-address-type-api-v1-address-types-id.md) — Deletes an address type from the system

## Auth

API endpoints for managing Auth resources.

- [POST /api/v1/auth/login](auth/post-login-api-v1-auth-login.md) — Login endpoint to get JWT token. Accepts both username and email address as identification.
- [POST /api/v1/auth/2fa/verify](auth/post-verify-mail-two-factor-api-v1-auth-2fa-verify.md) — Verifies a mail-based two-factor authentication code and issues JWT tokens.
- [POST /api/v1/auth/2fa/resend](auth/post-resend-mail-two-factor-api-v1-auth-2fa-resend.md) — Resends a mail-based two-factor authentication code for an active challenge.
- [POST /api/v1/auth/refresh](auth/post-refresh-token-api-v1-auth-refresh.md) — Refresh access token using refresh token
- [POST /api/v1/auth/logout](auth/post-logout-api-v1-auth-logout.md) — Logs out the current user by revoking their refresh token
- [GET /api/v1/auth/verify](auth/get-verify-token-api-v1-auth-verify.md) — Test endpoint to verify authentication is working

## Billing Cycles

Manages billing cycles including creation, retrieval, updates, and deletion

- [GET /api/v1/billing-cycles/{id}](billing-cycles/get-get-billing-cycle-by-id-api-v1-billing-cycles-id.md) — Manages billing cycles including creation, retrieval, updates, and deletion
- [POST /api/v1/billing-cycles](billing-cycles/post-create-billing-cycle-api-v1-billing-cycles.md) — Creates a new billing cycle in the system
- [PUT /api/v1/billing-cycles/{id}](billing-cycles/put-update-billing-cycle-api-v1-billing-cycles-id.md) — Updates an existing billing cycle's information
- [DELETE /api/v1/billing-cycles/{id}](billing-cycles/delete-delete-billing-cycle-api-v1-billing-cycles-id.md) — Deletes a billing cycle from the system

## Contact Persons

Manages contact persons for customers including creation, retrieval, updates, and deletion

- [GET /api/v1/contact-persons](contact-persons/get-get-all-contact-persons-api-v1-contact-persons.md) — Manages contact persons for customers including creation, retrieval, updates, and deletion
- [GET /api/v1/contact-persons/{id}](contact-persons/get-get-contact-person-by-id-api-v1-contact-persons-id.md) — Retrieves all contact persons that are available globally for domains
- [POST /api/v1/contact-persons](contact-persons/post-create-contact-person-api-v1-contact-persons.md) — Creates a new contact person in the system
- [PUT /api/v1/contact-persons/{id}](contact-persons/put-update-contact-person-api-v1-contact-persons-id.md) — Updates an existing contact person's information
- [PATCH /api/v1/contact-persons/{id}/domain-global](contact-persons/patch-patch-contact-person-is-domain-global-api-v1-contact-persons-id-domain-global.md) — Updates the IsDomainGlobal flag for an existing contact person
- [DELETE /api/v1/contact-persons/{id}](contact-persons/delete-delete-contact-person-api-v1-contact-persons-id.md) — Deletes a contact person from the system
- [GET /api/v1/contact-persons/customer/{customerId}/for-role/{roleType}](contact-persons/get-get-contact-persons-for-role-api-v1-contact-persons-customer-customerid-for-role-roletype.md) — Retrieves contact persons for a customer categorized by role preference and usage.     Returns a three-tiered list:     1. Preferred - Contact persons marked as default for the specified role     2. Frequently Used - Contact persons used 3+ times for the specified role     3. Available - All other contact persons

## Control Panel Types

Manages control panel types including creation, retrieval, updates, and deletion

- [GET /api/v1/control-panel-types/{id}](control-panel-types/get-get-control-panel-type-by-id-api-v1-control-panel-types-id.md) — Manages control panel types including creation, retrieval, updates, and deletion
- [POST /api/v1/control-panel-types](control-panel-types/post-create-control-panel-type-api-v1-control-panel-types.md) — Creates a new control panel type in the system
- [PUT /api/v1/control-panel-types/{id}](control-panel-types/put-update-control-panel-type-api-v1-control-panel-types-id.md) — Updates an existing control panel type's information
- [DELETE /api/v1/control-panel-types/{id}](control-panel-types/delete-delete-control-panel-type-api-v1-control-panel-types-id.md) — Deletes a control panel type from the system

## Countries

Manages countries and their information

- [POST /api/v1/countries/set-all-active/{isActive}](countries/post-set-all-countries-active-api-v1-countries-set-all-active-isactive.md) — Manages countries and their information
- [POST /api/v1/countries/set-active-by-codes](countries/post-set-countries-active-by-codes-api-v1-countries-set-active-by-codes.md) — Set active flag for a selection of countries by codes (comma separated or JSON array)
- [POST /api/v1/countries/upload-localized-names-csv](countries/post-upload-localized-names-csv-api-v1-countries-upload-localized-names-csv.md) — Upload a CSV file with localized country names to merge into the Country.LocalName field     Expected CSV columns: Iso2,LocalName
- [POST /api/v1/countries/upload-csv](countries/post-upload-countries-csv-api-v1-countries-upload-csv.md) — Upload a CSV file with countries to merge into the Country table
- [GET /api/v1/countries](countries/get-get-all-countries-api-v1-countries.md) — Retrieves all countries in the system
- [GET /api/v1/countries/active](countries/get-get-active-countries-api-v1-countries-active.md) — Retrieves only active countries
- [GET /api/v1/countries/{id}](countries/get-get-country-by-id-api-v1-countries-id.md) — Retrieves a specific country by its unique identifier
- [GET /api/v1/countries/code/{code}](countries/get-get-country-by-code-api-v1-countries-code-code.md) — Retrieves a specific country by its country code
- [POST /api/v1/countries](countries/post-create-country-api-v1-countries.md) — Creates a new country in the system
- [PUT /api/v1/countries/{id}](countries/put-update-country-api-v1-countries-id.md) — Update an existing country
- [DELETE /api/v1/countries/{id}](countries/delete-delete-country-api-v1-countries-id.md) — Delete a country

## Coupons

Manages discount coupons

- [GET /api/v1/coupons/{id}](coupons/get-get-coupon-by-id-api-v1-coupons-id.md) — Manages discount coupons
- [GET /api/v1/coupons/code/{code}](coupons/get-get-coupon-by-code-api-v1-coupons-code-code.md) — Retrieves a coupon by code
- [POST /api/v1/coupons](coupons/post-create-coupon-api-v1-coupons.md) — Creates a new coupon
- [PUT /api/v1/coupons/{id}](coupons/put-update-coupon-api-v1-coupons-id.md) — Updates an existing coupon
- [DELETE /api/v1/coupons/{id}](coupons/delete-delete-coupon-api-v1-coupons-id.md) — Deletes a coupon (soft delete)
- [POST /api/v1/coupons/validate](coupons/post-validate-coupon-api-v1-coupons-validate.md) — Retrieves paginated coupon usage entries

## Currencies

Manages currency exchange rates and currency conversions

- [GET /api/v1/currencies/rates/{id:int}](currencies/get-get-rate-by-id-api-v1-currencies-rates-id-int.md) — Manages currency exchange rates and currency conversions
- [GET /api/v1/currencies/rates/exchange](currencies/get-get-exchange-rate-api-v1-currencies-rates-exchange.md) — Retrieves the current exchange rate between two currencies
- [POST /api/v1/currencies/rates](currencies/post-create-rate-api-v1-currencies-rates.md) — Retrieves all exchange rates for a specific currency pair
- [PUT /api/v1/currencies/rates/{id:int}](currencies/put-update-rate-api-v1-currencies-rates-id-int.md) — Updates an existing currency exchange rate
- [POST /api/v1/currencies/rates/force-update](currencies/post-force-update-rates-api-v1-currencies-rates-force-update.md) — Forces an immediate download and update of exchange rates from the configured provider.
- [DELETE /api/v1/currencies/rates/{id:int}](currencies/delete-delete-rate-api-v1-currencies-rates-id-int.md) — Deletes a currency exchange rate
- [POST /api/v1/currencies/convert](currencies/post-convert-currency-api-v1-currencies-convert.md) — Converts an amount from one currency to another
- [POST /api/v1/currencies/rates/deactivate-expired](currencies/post-deactivate-expired-rates-api-v1-currencies-rates-deactivate-expired.md) — Deactivates all expired currency exchange rates

## Customer Addresses

Manages customer address information including creation, retrieval, updates, and deletion

- [GET /api/v1/customers/{customerId}/addresses](customer-addresses/get-get-customer-addresses-api-v1-customers-customerid-addresses.md) — Manages customer address information including creation, retrieval, updates, and deletion
- [GET /api/v1/customers/{customerId}/addresses/primary](customer-addresses/get-get-primary-address-api-v1-customers-customerid-addresses-primary.md) — Retrieves the primary address for a specific customer
- [GET /api/v1/customers/{customerId}/addresses/{id}](customer-addresses/get-get-customer-address-by-id-api-v1-customers-customerid-addresses-id.md) — Retrieves a specific customer address by its unique identifier
- [POST /api/v1/customers/{customerId}/addresses](customer-addresses/post-create-customer-address-api-v1-customers-customerid-addresses.md) — Creates a new customer address
- [PUT /api/v1/customers/{customerId}/addresses/{id}](customer-addresses/put-update-customer-address-api-v1-customers-customerid-addresses-id.md) — Updates an existing customer address
- [DELETE /api/v1/customers/{customerId}/addresses/{id}](customer-addresses/delete-delete-customer-address-api-v1-customers-customerid-addresses-id.md) — Deletes a customer address from the system
- [PUT /api/v1/customers/{customerId}/addresses/{id}/set-primary](customer-addresses/put-set-primary-address-api-v1-customers-customerid-addresses-id-set-primary.md) — Sets a customer address as the primary address

## Customer Credits

Manages customer credit balances and transactions

- [GET /api/v1/customer-credits/customer/{customerId}](customer-credits/get-get-customer-credit-api-v1-customer-credits-customer-customerid.md) — Manages customer credit balances and transactions
- [POST /api/v1/customer-credits/transactions](customer-credits/post-create-credit-transaction-api-v1-customer-credits-transactions.md) — Retrieves all credit transactions for a customer
- [POST /api/v1/customer-credits/customer/{customerId}/add](customer-credits/post-add-credit-api-v1-customer-credits-customer-customerid-add.md) — Adds credit to a customer account
- [POST /api/v1/customer-credits/customer/{customerId}/deduct](customer-credits/post-deduct-credit-api-v1-customer-credits-customer-customerid-deduct.md) — Deducts credit from a customer account
- [GET /api/v1/customer-credits/customer/{customerId}/check](customer-credits/get-has-sufficient-credit-api-v1-customer-credits-customer-customerid-check.md) — Checks if a customer has sufficient credit

## Customer Payment Methods

Manages customer payment methods

- [POST /api/v1/customer-payment-methods/mine](customer-payment-methods/post-create-my-payment-method-api-v1-customer-payment-methods-mine.md) — Manages customer payment methods
- [PUT /api/v1/customer-payment-methods/mine/{id}](customer-payment-methods/put-update-my-payment-method-api-v1-customer-payment-methods-mine-id.md) — PUT UpdateMyPaymentMethod
- [POST /api/v1/customer-payment-methods/mine/{id}/set-default](customer-payment-methods/post-set-my-default-payment-method-api-v1-customer-payment-methods-mine-id-set-default.md) — POST SetMyDefaultPaymentMethod
- [DELETE /api/v1/customer-payment-methods/mine/{id}](customer-payment-methods/delete-delete-my-payment-method-api-v1-customer-payment-methods-mine-id.md) — DELETE DeleteMyPaymentMethod
- [PUT /api/v1/customer-payment-methods/{id}](customer-payment-methods/put-update-payment-method-api-v1-customer-payment-methods-id.md) — Retrieves all payment methods for a customer
- [GET /api/v1/customer-payment-methods/{id}](customer-payment-methods/get-get-payment-method-by-id-api-v1-customer-payment-methods-id.md) — Retrieves a specific payment method by ID
- [GET /api/v1/customer-payment-methods/customer/{customerId}/default](customer-payment-methods/get-get-default-payment-method-api-v1-customer-payment-methods-customer-customerid-default.md) — Retrieves the default payment method for a customer
- [POST /api/v1/customer-payment-methods](customer-payment-methods/post-create-payment-method-api-v1-customer-payment-methods.md) — Creates a new payment method for a customer
- [POST /api/v1/customer-payment-methods/{id}/set-default](customer-payment-methods/post-set-as-default-api-v1-customer-payment-methods-id-set-default.md) — Sets a payment method as the default for a customer
- [DELETE /api/v1/customer-payment-methods/{id}](customer-payment-methods/delete-delete-payment-method-api-v1-customer-payment-methods-id.md) — Deletes a payment method

## Customers

Manages customer information including creation, retrieval, updates, and deletion

- [POST /api/v1/customers/{id}/internal-notes](customers/post-create-internal-note-api-v1-customers-id-internal-notes.md) — Manages customer information including creation, retrieval, updates, and deletion
- [GET /api/v1/customers](customers/get-get-all-customers-api-v1-customers.md) — Retrieves tracked changes for a customer.
- [GET /api/v1/customers/{id}](customers/get-get-customer-by-id-api-v1-customers-id.md) — Retrieves a specific customer by their unique identifier
- [POST /api/v1/customers](customers/post-create-customer-api-v1-customers.md) — Creates a new customer in the system
- [PUT /api/v1/customers/{id}](customers/put-update-customer-api-v1-customers-id.md) — Updates an existing customer's information
- [DELETE /api/v1/customers/{id}](customers/delete-delete-customer-api-v1-customers-id.md) — Deletes a customer from the system
- [GET /api/v1/customers/check-email](customers/get-check-email-exists-api-v1-customers-check-email.md) — Searches customers by a free-text query across name, customer name, email, phone,     reference number, customer number, and associated contact person details

## Customer Statuses

Manages customer statuses including creation, retrieval, updates, and deletion

- [GET /api/v1/customer-statuses/default](customer-statuses/get-get-default-customer-status-api-v1-customer-statuses-default.md) — Manages customer statuses including creation, retrieval, updates, and deletion
- [GET /api/v1/customer-statuses/{id}](customer-statuses/get-get-customer-status-by-id-api-v1-customer-statuses-id.md) — Retrieves a specific customer status by its unique identifier
- [GET /api/v1/customer-statuses/code/{code}](customer-statuses/get-get-customer-status-by-code-api-v1-customer-statuses-code-code.md) — Retrieves a specific customer status by its code
- [POST /api/v1/customer-statuses](customer-statuses/post-create-customer-status-api-v1-customer-statuses.md) — Creates a new customer status in the system
- [PUT /api/v1/customer-statuses/{id}](customer-statuses/put-update-customer-status-api-v1-customer-statuses-id.md) — Updates an existing customer status
- [DELETE /api/v1/customer-statuses/{id}](customer-statuses/delete-delete-customer-status-api-v1-customer-statuses-id.md) — Deletes a customer status

## Customer Tax Profiles

Manages customer tax profiles

- [GET /api/v1/customer-tax-profiles/{id}](customer-tax-profiles/get-get-customer-tax-profile-by-id-api-v1-customer-tax-profiles-id.md) — Manages customer tax profiles
- [GET /api/v1/customer-tax-profiles/customer/{customerId}](customer-tax-profiles/get-get-customer-tax-profile-by-customer-id-api-v1-customer-tax-profiles-customer-customerid.md) — Retrieves customer tax profile by customer ID
- [POST /api/v1/customer-tax-profiles](customer-tax-profiles/post-create-customer-tax-profile-api-v1-customer-tax-profiles.md) — Creates a new customer tax profile
- [PUT /api/v1/customer-tax-profiles/{id}](customer-tax-profiles/put-update-customer-tax-profile-api-v1-customer-tax-profiles-id.md) — Updates an existing customer tax profile
- [DELETE /api/v1/customer-tax-profiles/{id}](customer-tax-profiles/delete-delete-customer-tax-profile-api-v1-customer-tax-profiles-id.md) — Deletes a customer tax profile
- [POST /api/v1/customer-tax-profiles/validate](customer-tax-profiles/post-validate-tax-id-api-v1-customer-tax-profiles-validate.md) — Validates a customer's tax ID

## Dns Records

Manages DNS records for domains.

- [GET /api/v1/dns-records](dns-records/get-get-all-dns-records-api-v1-dns-records.md) — Manages DNS records for domains.
- [GET /api/v1/dns-records/{id}](dns-records/get-get-dns-record-by-id-api-v1-dns-records-id.md) — Retrieves a specific DNS record by its unique identifier.
- [GET /api/v1/dns-records/pending/count](dns-records/get-get-pending-sync-count-api-v1-dns-records-pending-count.md) — Retrieves all non-deleted DNS records for a specific domain.
- [POST /api/v1/dns-records](dns-records/post-create-dns-record-api-v1-dns-records.md) — Retrieves all DNS records of a specific type (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.).
- [PUT /api/v1/dns-records/{id}](dns-records/put-update-dns-record-api-v1-dns-records-id.md) — Updates an existing DNS record. The record is automatically marked as pending synchronisation.     System-managed records (IsEditableByUser = false) can only be edited by Admin or Support.
- [DELETE /api/v1/dns-records/{id}](dns-records/delete-delete-dns-record-api-v1-dns-records-id.md) — Soft-deletes a DNS record by flagging it as deleted and pending synchronisation.     The record is retained until hard-deleted after the removal is confirmed on the DNS server.
- [DELETE /api/v1/dns-records/{id}/hard](dns-records/delete-hard-delete-dns-record-api-v1-dns-records-id-hard.md) — Permanently removes a DNS record from the database.     Use this endpoint only after the deletion has been confirmed on the DNS server.
- [POST /api/v1/dns-records/{id}/restore](dns-records/post-restore-dns-record-api-v1-dns-records-id-restore.md) — Restores a soft-deleted DNS record and marks it as pending synchronisation.
- [POST /api/v1/dns-records/{id}/mark-synced](dns-records/post-mark-dns-record-as-synced-api-v1-dns-records-id-mark-synced.md) — Clears the pending-sync flag on a DNS record after it has been successfully pushed to the DNS server.
- [POST /api/v1/dns-records/{id}/push](dns-records/post-push-dns-record-api-v1-dns-records-id-push.md) — Pushes a single DNS record to the registrar's DNS server.     Non-deleted records are upserted (created or updated); soft-deleted records are removed     from the server and permanently deleted locally.     IsPendingSync is cleared on success.
- [POST /api/v1/dns-records/domain/{domainId}/push-pending](dns-records/post-push-pending-sync-dns-records-api-v1-dns-records-domain-domainid-push-pending.md) — Pushes all pending-sync DNS records for a domain to the registrar's DNS server.     Non-deleted records are upserted; soft-deleted records are removed from the server     and permanently deleted locally.

## Dns Record Types

Manages DNS record types (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.)

- [GET /api/v1/dns-record-types/{id}](dns-record-types/get-get-dns-record-type-by-id-api-v1-dns-record-types-id.md) — Manages DNS record types (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.)
- [GET /api/v1/dns-record-types/type/{type}](dns-record-types/get-get-dns-record-type-by-type-api-v1-dns-record-types-type-type.md) — Retrieves a specific DNS record type by its type name (e.g., A, AAAA, CNAME, MX, TXT)
- [POST /api/v1/dns-record-types](dns-record-types/post-create-dns-record-type-api-v1-dns-record-types.md) — Create a new DNS record type
- [PUT /api/v1/dns-record-types/{id}](dns-record-types/put-update-dns-record-type-api-v1-dns-record-types-id.md) — Update an existing DNS record type
- [DELETE /api/v1/dns-record-types/{id}](dns-record-types/delete-delete-dns-record-type-api-v1-dns-record-types-id.md) — Delete a DNS record type

## Dns Troubleshoot

Provides DNS troubleshooting checks for domains.

- [GET /api/v1/dns-troubleshoot/domain/{domainId:int}](dns-troubleshoot/get-run-for-domain-api-v1-dns-troubleshoot-domain-domainid-int.md) — Provides DNS troubleshooting checks for domains.

## Dns Zone Package Records

Manages DNS zone package records including creation, retrieval, updates, and deletion

- [GET /api/v1/dns-zone-package-records/{id}](dns-zone-package-records/get-get-dns-zone-package-record-by-id-api-v1-dns-zone-package-records-id.md) — Manages DNS zone package records including creation, retrieval, updates, and deletion
- [POST /api/v1/dns-zone-package-records](dns-zone-package-records/post-create-dns-zone-package-record-api-v1-dns-zone-package-records.md) — Creates a new DNS zone package record in the system
- [PUT /api/v1/dns-zone-package-records/{id}](dns-zone-package-records/put-update-dns-zone-package-record-api-v1-dns-zone-package-records-id.md) — Updates an existing DNS zone package record's information
- [DELETE /api/v1/dns-zone-package-records/{id}](dns-zone-package-records/delete-delete-dns-zone-package-record-api-v1-dns-zone-package-records-id.md) — Deletes a DNS zone package record from the system

## Dns Zone Packages

Manages DNS zone packages including creation, retrieval, updates, and deletion

- [GET /api/v1/dns-zone-packages/default](dns-zone-packages/get-get-default-dns-zone-package-api-v1-dns-zone-packages-default.md) — Manages DNS zone packages including creation, retrieval, updates, and deletion
- [GET /api/v1/dns-zone-packages/{id}](dns-zone-packages/get-get-dns-zone-package-by-id-api-v1-dns-zone-packages-id.md) — Retrieves a specific DNS zone package by its unique identifier
- [GET /api/v1/dns-zone-packages/{id}/with-records](dns-zone-packages/get-get-dns-zone-package-with-records-by-id-api-v1-dns-zone-packages-id-with-records.md) — Retrieves a specific DNS zone package with its records by its unique identifier
- [POST /api/v1/dns-zone-packages](dns-zone-packages/post-create-dns-zone-package-api-v1-dns-zone-packages.md) — Creates a new DNS zone package in the system
- [PUT /api/v1/dns-zone-packages/{id}](dns-zone-packages/put-update-dns-zone-package-api-v1-dns-zone-packages-id.md) — Updates an existing DNS zone package's information
- [DELETE /api/v1/dns-zone-packages/{id}](dns-zone-packages/delete-delete-dns-zone-package-api-v1-dns-zone-packages-id.md) — Deletes a DNS zone package from the system
- [POST /api/v1/dns-zone-packages/{packageId}/apply-to-domain/{domainId}](dns-zone-packages/post-apply-package-to-domain-api-v1-dns-zone-packages-packageid-apply-to-domain-domainid.md) — Applies a DNS zone package to a domain by creating DNS records
- [GET /api/v1/dns-zone-packages/{id}/assignments](dns-zone-packages/get-get-dns-zone-package-with-assignments-api-v1-dns-zone-packages-id-assignments.md) — GET GetDnsZonePackageWithAssignments
- [POST /api/v1/dns-zone-packages/{packageId}/control-panels/{controlPanelId}](dns-zone-packages/post-assign-control-panel-api-v1-dns-zone-packages-packageid-control-panels-controlpanelid.md) — POST AssignControlPanel
- [DELETE /api/v1/dns-zone-packages/{packageId}/control-panels/{controlPanelId}](dns-zone-packages/delete-remove-control-panel-api-v1-dns-zone-packages-packageid-control-panels-controlpanelid.md) — DELETE RemoveControlPanel
- [POST /api/v1/dns-zone-packages/{packageId}/servers/{serverId}](dns-zone-packages/post-assign-server-api-v1-dns-zone-packages-packageid-servers-serverid.md) — POST AssignServer
- [DELETE /api/v1/dns-zone-packages/{packageId}/servers/{serverId}](dns-zone-packages/delete-remove-server-api-v1-dns-zone-packages-packageid-servers-serverid.md) — DELETE RemoveServer

## Document Templates

Manages document templates for invoices, orders, emails, and other documents

- [GET /api/v1/document-templates/{id}](document-templates/get-get-template-by-id-api-v1-document-templates-id.md) — Manages document templates for invoices, orders, emails, and other documents
- [GET /api/v1/document-templates/default/{type}](document-templates/get-get-default-template-api-v1-document-templates-default-type.md) — Retrieves the default template for a specific type
- [GET /api/v1/document-templates/{id}/download](document-templates/get-download-template-api-v1-document-templates-id-download.md) — Downloads the file content of a specific document template
- [POST /api/v1/document-templates/upload](document-templates/post-upload-template-api-v1-document-templates-upload.md) — Uploads a new document template
- [PUT /api/v1/document-templates/{id}](document-templates/put-update-template-api-v1-document-templates-id.md) — Updates an existing document template's information and optionally replaces the file
- [PUT /api/v1/document-templates/{id}/set-default](document-templates/put-set-default-template-api-v1-document-templates-id-set-default.md) — Sets a document template as the default for its type
- [DELETE /api/v1/document-templates/{id}](document-templates/delete-delete-template-api-v1-document-templates-id.md) — Deletes a document template (soft delete)

## Domain Contacts

Manages domain contact persons for domain registrations including creation, retrieval, updates, and deletion

- [GET /api/v1/domain-contacts](domain-contacts/get-get-all-domain-contacts-api-v1-domain-contacts.md) — Manages domain contact persons for domain registrations including creation, retrieval, updates, and deletion
- [GET /api/v1/domain-contacts/{id}](domain-contacts/get-get-domain-contact-by-id-api-v1-domain-contacts-id.md) — Retrieves all domain contacts for a specific domain
- [POST /api/v1/domain-contacts](domain-contacts/post-create-domain-contact-api-v1-domain-contacts.md) — Creates a new domain contact in the system
- [PUT /api/v1/domain-contacts/{id}](domain-contacts/put-update-domain-contact-api-v1-domain-contacts-id.md) — Updates an existing domain contact's information
- [DELETE /api/v1/domain-contacts/{id}](domain-contacts/delete-delete-domain-contact-api-v1-domain-contacts-id.md) — Deletes a domain contact from the system
- [POST /api/v1/domain-contacts/migrate-to-contact-persons](domain-contacts/post-migrate-domain-contacts-to-contact-persons-api-v1-domain-contacts-migrate-to-contact-persons.md) — Migrates domain contacts to the normalized ContactPerson and DomainContactAssignment tables
- [GET /api/v1/domain-contacts/migration-needed](domain-contacts/get-is-migration-needed-api-v1-domain-contacts-migration-needed.md) — Checks if domain contact migration is needed
- [GET /api/v1/domain-contacts/migration-preview](domain-contacts/get-get-migration-preview-api-v1-domain-contacts-migration-preview.md) — Gets a preview of what the migration would do without performing it

## Domain Manager

Manages domain registration operations through various registrars

- [POST /api/v1/domain-manager/registrar/{registrarCode}/domain/{registeredDomainId}](domain-manager/post-register-domain-api-v1-domain-manager-registrar-registrarcode-domain-registereddomainid.md) — Manages domain registration operations through various registrars
- [GET /api/v1/domain-manager/registrar/{registrarCode}/domain/{registeredDomainId}/is-available](domain-manager/get-check-domain-availability-by-id-api-v1-domain-manager-registrar-registrarcode-domain-registereddomainid-is-available.md) — Checks if a domain is available for registration based on registered domain ID
- [GET /api/v1/domain-manager/registrar/{registrarCode}/domain/name/{domainName}/is-available](domain-manager/get-check-domain-availability-by-name-api-v1-domain-manager-registrar-registrarcode-domain-name-domainname-is-available.md) — Checks if a domain is available for registration based on domain name
- [GET /api/v1/domain-manager/domain/name/{domainName}/alternatives](domain-manager/get-get-alternative-domain-names-api-v1-domain-manager-domain-name-domainname-alternatives.md) — Generates alternative domain name suggestions based on active TLDs and variations of the provided name.
- [POST /api/v1/domain-manager/registrar/{registrarCode}/domain/name/{domainName}/dns-records/sync](domain-manager/post-sync-dns-records-for-domain-api-v1-domain-manager-registrar-registrarcode-domain-name-domainname-dns-records-sync.md) — Downloads DNS records from the registrar for a single domain and merges them into the local database
- [POST /api/v1/domain-manager/registrar/{registrarCode}/dns-records/sync](domain-manager/post-sync-dns-records-for-all-domains-api-v1-domain-manager-registrar-registrarcode-dns-records-sync.md) — Downloads DNS records from the registrar for all domains assigned to that registrar     and merges them into the local database
- [GET /api/v1/domain-manager/registrar/default/domain/{domainName}/is-available](domain-manager/get-check-domain-availability-by-name-api-v1-domain-manager-registrar-default-domain-domainname-is-available.md) — Checks if a domain is available for registration based on domain name

## Email Queue

Manages email queue operations for sending emails asynchronously

- [POST /api/v1/email-queue/queue](email-queue/post-queue-email-api-v1-email-queue-queue.md) — Manages email queue operations for sending emails asynchronously
- [GET /api/v1/email-queue/status/{id}](email-queue/get-get-email-status-api-v1-email-queue-status-id.md) — Gets the status of a queued email
- [POST /api/v1/email-queue/retry/{id}](email-queue/post-retry-email-api-v1-email-queue-retry-id.md) — Queues an existing failed or pending email for immediate retry.
- [POST /api/v1/email-queue/events/provider](email-queue/post-apply-provider-event-api-v1-email-queue-events-provider.md) — Applies a provider delivery event to email queue and communication status.

## Email Receiver

Provides endpoints for reading inbound emails through configured receiver plugins.

- [POST /api/v1/email-receiver/token/test](email-receiver/post-test-office365-token-api-v1-email-receiver-token-test.md) — Provides endpoints for reading inbound emails through configured receiver plugins.
- [GET /api/v1/email-receiver/messages](email-receiver/get-read-messages-api-v1-email-receiver-messages.md) — Reads messages from the configured Office365 mailbox.
- [PATCH /api/v1/email-receiver/messages/{externalMessageId}/read](email-receiver/patch-mark-message-as-read-api-v1-email-receiver-messages-externalmessageid-read.md) — Marks a mailbox message as read by external message identifier.
- [GET /api/v1/email-receiver/diagnostics](email-receiver/get-diagnostics-api-v1-email-receiver-diagnostics.md) — Temporary diagnostics endpoint for Office365 receiver configuration and token claims.

## Communication Threads

Provides read operations for communication threads.

- [GET /api/v1/communication-threads](communication-threads/get-get-threads-api-v1-communication-threads.md) — Retrieves communication threads with optional filters.
- [GET /api/v1/communication-threads/{id:int}](communication-threads/get-get-thread-by-id-api-v1-communication-threads-id-int.md) — Retrieves a communication thread by identifier including participants and messages.
- [PATCH /api/v1/communication-threads/{id:int}/status](communication-threads/patch-update-thread-status-api-v1-communication-threads-id-int-status.md) — Updates the status of a communication thread.
- [PATCH /api/v1/communication-threads/messages/{messageId:int}/read-state](communication-threads/patch-update-message-read-state-api-v1-communication-threads-messages-messageid-int-read-state.md) — Updates the read state of a communication message.
- [POST /api/v1/communication-threads/{id:int}/reply](communication-threads/post-queue-reply-api-v1-communication-threads-id-int-reply.md) — Queues a reply email for a communication thread.

## Exchange Rate Download Logs

Controller for managing exchange rate download logs

- [GET /api/exchange-rate-download-logs/{id}](exchange-rate-download-logs/get-get-log-by-id-api-exchange-rate-download-logs-id.md) — Controller for managing exchange rate download logs
- [GET /api/exchange-rate-download-logs/summary](exchange-rate-download-logs/get-get-summary-api-exchange-rate-download-logs-summary.md) — Gets exchange rate download logs by source/provider
- [POST /api/exchange-rate-download-logs](exchange-rate-download-logs/post-create-log-api-exchange-rate-download-logs.md) — Gets failed exchange rate download logs
- [GET /api/exchange-rate-download-logs/last-download/{baseCurrency}/{source}](exchange-rate-download-logs/get-get-last-download-api-exchange-rate-download-logs-last-download-basecurrency-source.md) — Gets the last successful download for a specific currency pair and source

## Hosting Accounts

Manages hosting accounts including creation, retrieval, updates, and synchronization

- [GET /api/v1/hosting-accounts/{id}](hosting-accounts/get-get-hosting-account-api-v1-hosting-accounts-id.md) — Manages hosting accounts including creation, retrieval, updates, and synchronization
- [GET /api/v1/hosting-accounts/{id}/details](hosting-accounts/get-get-hosting-account-with-details-api-v1-hosting-accounts-id-details.md) — Retrieves a hosting account with full details including domains, emails, databases, and FTP accounts
- [POST /api/v1/hosting-accounts](hosting-accounts/post-create-hosting-account-api-v1-hosting-accounts.md) — Retrieves all hosting accounts for a specific customer
- [POST /api/v1/hosting-accounts/create-and-sync](hosting-accounts/post-create-hosting-account-and-sync-api-v1-hosting-accounts-create-and-sync.md) — Creates a new hosting account and synchronizes it to the server
- [PUT /api/v1/hosting-accounts/{id}](hosting-accounts/put-update-hosting-account-api-v1-hosting-accounts-id.md) — Updates an existing hosting account
- [DELETE /api/v1/hosting-accounts/{id}](hosting-accounts/delete-delete-hosting-account-api-v1-hosting-accounts-id.md) — Deletes a hosting account
- [GET /api/v1/hosting-accounts/{id}/resource-usage](hosting-accounts/get-get-resource-usage-api-v1-hosting-accounts-id-resource-usage.md) — Gets resource usage statistics for a hosting account
- [GET /api/v1/hosting-accounts/{id}/sync-status](hosting-accounts/get-get-sync-status-api-v1-hosting-accounts-id-sync-status.md) — Gets sync status for a hosting account
- [POST /api/v1/hosting-accounts/{id}/provision-on-cpanel](hosting-accounts/post-provision-account-on-c-panel-api-v1-hosting-accounts-id-provision-on-cpanel.md) — Provisions a hosting account on CPanel using a domain from the HostingDomains table

## Hosting Databases

Manages databases and database users for hosting accounts

- [GET /api/v1/hosting-accounts/{hostingAccountId}/databases/{id}](hosting-databases/get-get-database-api-v1-hosting-accounts-hostingaccountid-databases-id.md) — Manages databases and database users for hosting accounts
- [POST /api/v1/hosting-accounts/{hostingAccountId}/databases](hosting-databases/post-create-database-api-v1-hosting-accounts-hostingaccountid-databases.md) — POST CreateDatabase
- [DELETE /api/v1/hosting-accounts/{hostingAccountId}/databases/{id}](hosting-databases/delete-delete-database-api-v1-hosting-accounts-hostingaccountid-databases-id.md) — DELETE DeleteDatabase
- [POST /api/v1/hosting-accounts/{hostingAccountId}/databases/{databaseId}/users](hosting-databases/post-create-database-user-api-v1-hosting-accounts-hostingaccountid-databases-databaseid-users.md) — POST CreateDatabaseUser
- [DELETE /api/v1/hosting-accounts/{hostingAccountId}/databases/{databaseId}/users/{userId}](hosting-databases/delete-delete-database-user-api-v1-hosting-accounts-hostingaccountid-databases-databaseid-users-userid.md) — DELETE DeleteDatabaseUser
- [POST /api/v1/hosting-accounts/{hostingAccountId}/databases/sync](hosting-databases/post-sync-databases-from-server-api-v1-hosting-accounts-hostingaccountid-databases-sync.md) — POST SyncDatabasesFromServer

## Hosting Domains

Manages domains for hosting accounts (main, addon, parked, subdomains)

- [GET /api/v1/hosting-accounts/{hostingAccountId}/domains/{id}](hosting-domains/get-get-domain-api-v1-hosting-accounts-hostingaccountid-domains-id.md) — Manages domains for hosting accounts (main, addon, parked, subdomains)
- [POST /api/v1/hosting-accounts/{hostingAccountId}/domains](hosting-domains/post-create-domain-api-v1-hosting-accounts-hostingaccountid-domains.md) — Creates a new domain for a hosting account
- [PUT /api/v1/hosting-accounts/{hostingAccountId}/domains/{id}](hosting-domains/put-update-domain-api-v1-hosting-accounts-hostingaccountid-domains-id.md) — Updates a domain
- [DELETE /api/v1/hosting-accounts/{hostingAccountId}/domains/{id}](hosting-domains/delete-delete-domain-api-v1-hosting-accounts-hostingaccountid-domains-id.md) — Deletes a domain
- [POST /api/v1/hosting-accounts/{hostingAccountId}/domains/sync](hosting-domains/post-sync-domains-from-server-api-v1-hosting-accounts-hostingaccountid-domains-sync.md) — Synchronizes domains from the hosting server to the database

## Hosting Email

Manages email accounts for hosting accounts

- [GET /api/v1/hosting-accounts/{hostingAccountId}/emails/{id}](hosting-email/get-get-email-account-api-v1-hosting-accounts-hostingaccountid-emails-id.md) — Manages email accounts for hosting accounts
- [POST /api/v1/hosting-accounts/{hostingAccountId}/emails](hosting-email/post-create-email-account-api-v1-hosting-accounts-hostingaccountid-emails.md) — POST CreateEmailAccount
- [PUT /api/v1/hosting-accounts/{hostingAccountId}/emails/{id}](hosting-email/put-update-email-account-api-v1-hosting-accounts-hostingaccountid-emails-id.md) — PUT UpdateEmailAccount
- [DELETE /api/v1/hosting-accounts/{hostingAccountId}/emails/{id}](hosting-email/delete-delete-email-account-api-v1-hosting-accounts-hostingaccountid-emails-id.md) — DELETE DeleteEmailAccount
- [POST /api/v1/hosting-accounts/{hostingAccountId}/emails/{id}/change-password](hosting-email/post-change-email-password-api-v1-hosting-accounts-hostingaccountid-emails-id-change-password.md) — POST ChangeEmailPassword
- [POST /api/v1/hosting-accounts/{hostingAccountId}/emails/sync](hosting-email/post-sync-email-accounts-from-server-api-v1-hosting-accounts-hostingaccountid-emails-sync.md) — POST SyncEmailAccountsFromServer

## Hosting Ftp

Manages FTP accounts for hosting accounts

- [GET /api/v1/hosting-accounts/{hostingAccountId}/ftp/{id}](hosting-ftp/get-get-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp-id.md) — Manages FTP accounts for hosting accounts
- [POST /api/v1/hosting-accounts/{hostingAccountId}/ftp](hosting-ftp/post-create-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp.md) — POST CreateFtpAccount
- [PUT /api/v1/hosting-accounts/{hostingAccountId}/ftp/{id}](hosting-ftp/put-update-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp-id.md) — PUT UpdateFtpAccount
- [DELETE /api/v1/hosting-accounts/{hostingAccountId}/ftp/{id}](hosting-ftp/delete-delete-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp-id.md) — DELETE DeleteFtpAccount
- [POST /api/v1/hosting-accounts/{hostingAccountId}/ftp/{id}/change-password](hosting-ftp/post-change-ftp-password-api-v1-hosting-accounts-hostingaccountid-ftp-id-change-password.md) — POST ChangeFtpPassword
- [POST /api/v1/hosting-accounts/{hostingAccountId}/ftp/sync](hosting-ftp/post-sync-ftp-accounts-from-server-api-v1-hosting-accounts-hostingaccountid-ftp-sync.md) — POST SyncFtpAccountsFromServer

## Hosting Packages

Manages hosting packages including creation, retrieval, updates, and deletion

- [GET /api/v1/hosting-packages/{id}](hosting-packages/get-get-hosting-package-by-id-api-v1-hosting-packages-id.md) — Manages hosting packages including creation, retrieval, updates, and deletion
- [POST /api/v1/hosting-packages](hosting-packages/post-create-hosting-package-api-v1-hosting-packages.md) — Creates a new hosting package in the system
- [PUT /api/v1/hosting-packages/{id}](hosting-packages/put-update-hosting-package-api-v1-hosting-packages-id.md) — Updates an existing hosting package's information
- [DELETE /api/v1/hosting-packages/{id}](hosting-packages/delete-delete-hosting-package-api-v1-hosting-packages-id.md) — Deletes a hosting package from the system

## Hosting Sync

Manages synchronization between database and hosting panel servers

- [POST /api/v1/hosting-sync/import](hosting-sync/post-import-account-from-server-api-v1-hosting-sync-import.md) — Manages synchronization between database and hosting panel servers
- [POST /api/v1/hosting-sync/export/{hostingAccountId}](hosting-sync/post-export-account-to-server-api-v1-hosting-sync-export-hostingaccountid.md) — Exports a hosting account from the database to the server
- [POST /api/v1/hosting-sync/import-all/{serverControlPanelId}](hosting-sync/post-import-all-accounts-from-server-api-v1-hosting-sync-import-all-servercontrolpanelid.md) — Imports all hosting accounts from a server to the database
- [GET /api/v1/hosting-sync/compare/{hostingAccountId}](hosting-sync/get-compare-with-server-api-v1-hosting-sync-compare-hostingaccountid.md) — Compares a hosting account in the database with its state on the server

## Host Providers

Manages host providers including creation, retrieval, updates, and deletion

- [GET /api/v1/host-providers/{id}](host-providers/get-get-host-provider-by-id-api-v1-host-providers-id.md) — Manages host providers including creation, retrieval, updates, and deletion
- [POST /api/v1/host-providers](host-providers/post-create-host-provider-api-v1-host-providers.md) — Creates a new host provider in the system
- [PUT /api/v1/host-providers/{id}](host-providers/put-update-host-provider-api-v1-host-providers-id.md) — Updates an existing host provider's information
- [DELETE /api/v1/host-providers/{id}](host-providers/delete-delete-host-provider-api-v1-host-providers-id.md) — Deletes a host provider from the system

## Initialization

Handles system initialization with the first admin user

- [POST /api/v1/initialization/import-customer-snapshot](initialization/post-import-customer-snapshot-api-v1-initialization-import-customer-snapshot.md) — Handles system initialization with the first admin user
- [GET /api/v1/initialization/status](initialization/get-status-api-v1-initialization-status.md) — Gets whether the database has already been initialized.
- [GET /api/v1/initialization/build-mode](initialization/get-get-build-mode-api-v1-initialization-build-mode.md) — Gets whether the API runs in Debug mode for initialization-time debug tooling.
- [POST /api/v1/initialization/import-admin-mycompany-snapshot](initialization/post-import-admin-my-company-snapshot-api-v1-initialization-import-admin-mycompany-snapshot.md) — Imports admin user and MyCompany profile from a debug snapshot file.
- [POST /api/v1/initialization/initialize-admin](initialization/post-initialize-admin-api-v1-initialization-initialize-admin.md) — Initializes the system with the first admin user (only works if no users exist)
- [POST /api/v1/initialization/initialize-customer](initialization/post-initialize-customer-api-v1-initialization-initialize-customer.md) — Initializes the user panel with the first customer user, company and primary contact person.

## Invoice Lines

Manages invoice line items representing individual charges on invoices

- [GET /api/v1/invoice-lines](invoice-lines/get-get-all-invoice-lines-api-v1-invoice-lines.md) — Manages invoice line items representing individual charges on invoices
- [GET /api/v1/invoice-lines/{id}](invoice-lines/get-get-invoice-line-by-id-api-v1-invoice-lines-id.md) — Retrieves a specific invoice line by its unique identifier
- [POST /api/v1/invoice-lines](invoice-lines/post-create-invoice-line-api-v1-invoice-lines.md) — Retrieves all line items for a specific invoice
- [PUT /api/v1/invoice-lines/{id}](invoice-lines/put-update-invoice-line-api-v1-invoice-lines-id.md) — Update an existing invoice line
- [DELETE /api/v1/invoice-lines/{id}](invoice-lines/delete-delete-invoice-line-api-v1-invoice-lines-id.md) — Delete an invoice line

## Invoices

Manages customer invoices including creation, retrieval, updates, and deletion

- [GET /api/v1/invoices](invoices/get-get-all-invoices-api-v1-invoices.md) — Manages customer invoices including creation, retrieval, updates, and deletion
- [GET /api/v1/invoices/customer/{customerId}](invoices/get-get-invoices-by-customer-id-api-v1-invoices-customer-customerid.md) — Retrieves all invoices for a specific customer
- [GET /api/v1/invoices/{id}](invoices/get-get-invoice-by-id-api-v1-invoices-id.md) — Retrieves all invoices with a specific status
- [GET /api/v1/invoices/number/{invoiceNumber}](invoices/get-get-invoice-by-number-api-v1-invoices-number-invoicenumber.md) — Retrieves a specific invoice by its invoice number
- [POST /api/v1/invoices](invoices/post-create-invoice-api-v1-invoices.md) — Creates a new invoice in the system
- [PUT /api/v1/invoices/{id}](invoices/put-update-invoice-api-v1-invoices-id.md) — Updates an existing invoice's information
- [DELETE /api/v1/invoices/{id}](invoices/delete-delete-invoice-api-v1-invoices-id.md) — Delete an invoice (soft delete)

## Login Histories

Controller for viewing login history entries.

- [GET /api/v1/login-histories](login-histories/get-get-all-api-v1-login-histories.md) — Controller for viewing login history entries.
- [GET /api/v1/login-histories/{id}](login-histories/get-get-by-id-api-v1-login-histories-id.md) — Retrieves a specific login history entry by identifier.

## My Account

Manages user account operations including registration, email confirmation, and password management

- [POST /api/v1/my-account/register](my-account/post-register-api-v1-my-account-register.md) — Manages user account operations including registration, email confirmation, and password management
- [POST /api/v1/my-account/confirm-email](my-account/post-confirm-email-api-v1-my-account-confirm-email.md) — Confirms user email address using the confirmation token sent during registration
- [POST /api/v1/my-account/request-email-confirmation](my-account/post-request-email-confirmation-api-v1-my-account-request-email-confirmation.md) — Requests a new email confirmation link for the currently authenticated user.
- [GET /api/v1/my-account/2fa/status](my-account/get-get-two-factor-status-api-v1-my-account-2fa-status.md) — Gets current mail-based two-factor authentication status for the authenticated user.
- [POST /api/v1/my-account/2fa](my-account/post-update-two-factor-setting-api-v1-my-account-2fa.md) — Updates mail-based two-factor authentication setting for the authenticated user.
- [DELETE /api/v1/my-account/2fa](my-account/delete-delete-two-factor-api-v1-my-account-2fa.md) — Deletes all two-factor authentication settings for the authenticated user.
- [POST /api/v1/my-account/2fa/authenticator/setup](my-account/post-begin-authenticator-setup-api-v1-my-account-2fa-authenticator-setup.md) — Starts Microsoft Authenticator setup by generating a shared key and QR provisioning URI.
- [POST /api/v1/my-account/2fa/authenticator/confirm](my-account/post-confirm-authenticator-setup-api-v1-my-account-2fa-authenticator-confirm.md) — Confirms Microsoft Authenticator setup using a current verification code.
- [POST /api/v1/my-account/request-password-reset](my-account/post-request-password-reset-api-v1-my-account-request-password-reset.md) — Requests a password reset by sending an email with a reset token
- [POST /api/v1/my-account/reset-password](my-account/post-reset-password-api-v1-my-account-reset-password.md) — Resets password using password reset token (no email required)
- [POST /api/v1/my-account/set-password](my-account/post-set-password-api-v1-my-account-set-password.md) — Sets password for a new account or after password reset using a token
- [POST /api/v1/my-account/change-password](my-account/post-change-password-api-v1-my-account-change-password.md) — Changes password for the currently authenticated user
- [PATCH /api/v1/my-account/email](my-account/patch-patch-email-api-v1-my-account-email.md) — Updates the email address for the currently authenticated user
- [PATCH /api/v1/my-account/customer](my-account/patch-patch-customer-info-api-v1-my-account-customer.md) — Update customer information for authenticated user
- [GET /api/v1/my-account/me](my-account/get-get-my-account-api-v1-my-account-me.md) — Get current authenticated user's account information

## My Company

Manages the reseller's own company profile used in invoices, mail templates, and letterheads.

- [GET /api/v1/my-company](my-company/get-get-my-company-api-v1-my-company.md) — Manages the reseller's own company profile used in invoices, mail templates, and letterheads.
- [PUT /api/v1/my-company](my-company/put-upsert-my-company-api-v1-my-company.md) — Creates or updates the company profile.

## Name Servers

Manages name servers for domains

- [GET /api/v1/name-servers](name-servers/get-get-all-name-servers-api-v1-name-servers.md) — Manages name servers for domains
- [GET /api/v1/name-servers/{id}](name-servers/get-get-name-server-by-id-api-v1-name-servers-id.md) — Retrieves a specific name server by its unique identifier
- [POST /api/v1/name-servers](name-servers/post-create-name-server-api-v1-name-servers.md) — Retrieves all name servers for a specific domain
- [PUT /api/v1/name-servers/{id}](name-servers/put-update-name-server-api-v1-name-servers-id.md) — Updates an existing name server
- [DELETE /api/v1/name-servers/{id}](name-servers/delete-delete-name-server-api-v1-name-servers-id.md) — Deletes a name server

## Operating Systems

Manages operating systems including creation, retrieval, updates, and deletion

- [GET /api/v1/operating-systems/{id}](operating-systems/get-get-operating-system-by-id-api-v1-operating-systems-id.md) — Manages operating systems including creation, retrieval, updates, and deletion
- [POST /api/v1/operating-systems](operating-systems/post-create-operating-system-api-v1-operating-systems.md) — Creates a new operating system in the system
- [PUT /api/v1/operating-systems/{id}](operating-systems/put-update-operating-system-api-v1-operating-systems-id.md) — Updates an existing operating system's information
- [DELETE /api/v1/operating-systems/{id}](operating-systems/delete-delete-operating-system-api-v1-operating-systems-id.md) — Deletes an operating system from the system

## Orders

Manages customer orders including creation, retrieval, updates, and deletion

- [POST /api/v1/orders/checkout/{id:int}/cancel](orders/post-cancel-checkout-order-api-v1-orders-checkout-id-int-cancel.md) — Manages customer orders including creation, retrieval, updates, and deletion
- [POST /api/v1/orders/checkout](orders/post-create-checkout-order-api-v1-orders-checkout.md) — Creates a new checkout order for the currently authenticated user.
- [GET /api/v1/orders/{id}](orders/get-get-order-by-id-api-v1-orders-id.md) — Retrieves all orders in the system
- [POST /api/v1/orders](orders/post-create-order-api-v1-orders.md) — Creates a new order in the system
- [PUT /api/v1/orders/{id}](orders/put-update-order-api-v1-orders-id.md) — Updates an existing order's information
- [DELETE /api/v1/orders/{id}](orders/delete-delete-order-api-v1-orders-id.md) — Deletes an order from the system

## Order Tax Snapshots

Manages immutable order tax snapshots.

- [GET /api/v1/order-tax-snapshots/{id}](order-tax-snapshots/get-get-order-tax-snapshot-by-id-api-v1-order-tax-snapshots-id.md) — Manages immutable order tax snapshots.
- [POST /api/v1/order-tax-snapshots](order-tax-snapshots/post-create-order-tax-snapshot-api-v1-order-tax-snapshots.md) — Creates an order tax snapshot.
- [PUT /api/v1/order-tax-snapshots/{id}](order-tax-snapshots/put-update-order-tax-snapshot-api-v1-order-tax-snapshots-id.md) — Updates an order tax snapshot.
- [DELETE /api/v1/order-tax-snapshots/{id}](order-tax-snapshots/delete-delete-order-tax-snapshot-api-v1-order-tax-snapshots-id.md) — Deletes an order tax snapshot.

## Payment Gateways

Manages payment gateway configurations including creation, retrieval, updates, and deletion

- [GET /api/v1/payment-gateways/default](payment-gateways/get-get-default-payment-gateway-api-v1-payment-gateways-default.md) — Manages payment gateway configurations including creation, retrieval, updates, and deletion
- [GET /api/v1/payment-gateways/{id}](payment-gateways/get-get-payment-gateway-by-id-api-v1-payment-gateways-id.md) — Retrieves a specific payment gateway by its unique identifier
- [GET /api/v1/payment-gateways/provider/{providerCode}](payment-gateways/get-get-payment-gateway-by-provider-api-v1-payment-gateways-provider-providercode.md) — Retrieves a payment gateway by provider code
- [POST /api/v1/payment-gateways](payment-gateways/post-create-payment-gateway-api-v1-payment-gateways.md) — Creates a new payment gateway
- [PUT /api/v1/payment-gateways/{id}](payment-gateways/put-update-payment-gateway-api-v1-payment-gateways-id.md) — Updates an existing payment gateway
- [POST /api/v1/payment-gateways/{id}/set-default](payment-gateways/post-set-default-payment-gateway-api-v1-payment-gateways-id-set-default.md) — Sets a payment gateway as the default
- [POST /api/v1/payment-gateways/{id}/set-active](payment-gateways/post-set-payment-gateway-active-status-api-v1-payment-gateways-id-set-active.md) — Activates or deactivates a payment gateway
- [DELETE /api/v1/payment-gateways/{id}](payment-gateways/delete-delete-payment-gateway-api-v1-payment-gateways-id.md) — Deletes a payment gateway (soft delete)

## Payment Instruments

API endpoints for managing PaymentInstruments resources.

- [GET /api/v1/payment-instruments/{id}](payment-instruments/get-get-by-id-api-v1-payment-instruments-id.md) — GET GetById
- [POST /api/v1/payment-instruments](payment-instruments/post-create-api-v1-payment-instruments.md) — POST Create
- [PUT /api/v1/payment-instruments/{id}](payment-instruments/put-update-api-v1-payment-instruments-id.md) — PUT Update
- [DELETE /api/v1/payment-instruments/{id}](payment-instruments/delete-delete-api-v1-payment-instruments-id.md) — DELETE Delete

## Payment Intents

Manages payment intents for processing payments

- [GET /api/v1/payment-intents/{id}](payment-intents/get-get-payment-intent-by-id-api-v1-payment-intents-id.md) — Manages payment intents for processing payments
- [POST /api/v1/payment-intents](payment-intents/post-create-payment-intent-api-v1-payment-intents.md) — Retrieves all payment intents for a specific customer
- [POST /api/v1/payment-intents/{id}/confirm](payment-intents/post-confirm-payment-intent-api-v1-payment-intents-id-confirm.md) — Confirms a payment intent with a payment method
- [POST /api/v1/payment-intents/{id}/cancel](payment-intents/post-cancel-payment-intent-api-v1-payment-intents-id-cancel.md) — Cancels a payment intent
- [POST /api/v1/payment-intents/webhook/{gatewayId}](payment-intents/post-process-webhook-api-v1-payment-intents-webhook-gatewayid.md) — Processes a webhook from a payment gateway

## Payments

Manages payment processing operations

- [POST /api/v1/payments/process](payments/post-process-invoice-payment-api-v1-payments-process.md) — Manages payment processing operations
- [POST /api/v1/payments/apply-credit](payments/post-apply-customer-credit-api-v1-payments-apply-credit.md) — Applies customer credit to an invoice
- [POST /api/v1/payments/partial](payments/post-process-partial-payment-api-v1-payments-partial.md) — Processes a partial payment
- [POST /api/v1/payments/retry/{paymentAttemptId}](payments/post-retry-failed-payment-api-v1-payments-retry-paymentattemptid.md) — Retries a failed payment
- [POST /api/v1/payments/confirm-authentication/{paymentAttemptId}](payments/post-confirm-authentication-api-v1-payments-confirm-authentication-paymentattemptid.md) — Confirms payment authentication (3D Secure)
- [GET /api/v1/payments/attempts/{id}](payments/get-get-payment-attempt-by-id-api-v1-payments-attempts-id.md) — Gets payment attempts for an invoice
- [POST /api/v1/payments/webhook/{gatewayName}](payments/post-handle-webhook-api-v1-payments-webhook-gatewayname.md) — Handles payment gateway webhooks

## Postal Codes

Manages postal codes and their geographic information

- [GET /api/v1/postal-codes](postal-codes/get-get-all-postal-codes-api-v1-postal-codes.md) — Manages postal codes and their geographic information
- [GET /api/v1/postal-codes/lookup](postal-codes/get-lookup-postal-code-api-v1-postal-codes-lookup.md) — Lookup a postal code by country code and postal code combination
- [GET /api/v1/postal-codes/{id}](postal-codes/get-get-postal-code-by-id-api-v1-postal-codes-id.md) — Retrieves all postal codes for a specific country
- [GET /api/v1/postal-codes/{code}/country/{countryCode}](postal-codes/get-get-postal-code-by-code-and-country-api-v1-postal-codes-code-country-countrycode.md) — Retrieves a specific postal code by code and country
- [POST /api/v1/postal-codes](postal-codes/post-create-postal-code-api-v1-postal-codes.md) — Create a new postal code
- [PUT /api/v1/postal-codes/{id}](postal-codes/put-update-postal-code-api-v1-postal-codes-id.md) — Update an existing postal code
- [DELETE /api/v1/postal-codes/{id}](postal-codes/delete-delete-postal-code-api-v1-postal-codes-id.md) — Delete a postal code
- [POST /api/v1/postal-codes/upload-csv](postal-codes/post-upload-postal-codes-csv-api-v1-postal-codes-upload-csv.md) — Upload a CSV file with postal codes to merge into the PostalCodes table for a specific country.     Expected CSV format: PostalCode,City,State,Region,District,Latitude,Longitude,IsActive (minimum: PostalCode,City)

## Profit Margin Settings

Manages profit margin settings for product classes.

- [GET /api/v1/profit-margin-settings/{id:int}](profit-margin-settings/get-get-by-id-api-v1-profit-margin-settings-id-int.md) — Manages profit margin settings for product classes.
- [GET /api/v1/profit-margin-settings/by-class/{productClass}](profit-margin-settings/get-get-by-product-class-api-v1-profit-margin-settings-by-class-productclass.md) — Retrieves active profit margin setting for a product class.
- [POST /api/v1/profit-margin-settings](profit-margin-settings/post-create-api-v1-profit-margin-settings.md) — Creates a new profit margin setting.
- [PUT /api/v1/profit-margin-settings/{id:int}](profit-margin-settings/put-update-api-v1-profit-margin-settings-id-int.md) — Updates an existing profit margin setting.
- [DELETE /api/v1/profit-margin-settings/{id:int}](profit-margin-settings/delete-delete-api-v1-profit-margin-settings-id-int.md) — Deletes a profit margin setting.

## Quotes

Manages sales quotes and proposals

- [GET /api/v1/quotes/{id}](quotes/get-get-quote-by-id-api-v1-quotes-id.md) — Manages sales quotes and proposals
- [POST /api/v1/quotes](quotes/post-create-quote-api-v1-quotes.md) — Retrieves all quotes for a specific customer
- [PUT /api/v1/quotes/{id}](quotes/put-update-quote-api-v1-quotes-id.md) — Updates an existing quote
- [DELETE /api/v1/quotes/{id}](quotes/delete-delete-quote-api-v1-quotes-id.md) — Deletes a quote (soft delete)
- [POST /api/v1/quotes/{id}/send](quotes/post-send-quote-api-v1-quotes-id-send.md) — Sends a quote to the customer via email
- [POST /api/v1/quotes/accept/{token}](quotes/post-accept-quote-api-v1-quotes-accept-token.md) — Accepts a quote using the acceptance token (public endpoint for customers)
- [POST /api/v1/quotes/{id}/reject](quotes/post-reject-quote-api-v1-quotes-id-reject.md) — Rejects a quote
- [POST /api/v1/quotes/{id}/convert](quotes/post-convert-quote-to-order-api-v1-quotes-id-convert.md) — Converts a quote to an order
- [GET /api/v1/quotes/{id}/pdf](quotes/get-generate-quote-pdf-api-v1-quotes-id-pdf.md) — Generates a PDF for the quote

## Refund Loss Audits

Manages refund loss audits

- [GET /api/v1/refund-loss-audits/{id}](refund-loss-audits/get-get-refund-loss-audit-by-id-api-v1-refund-loss-audits-id.md) — Manages refund loss audits
- [GET /api/v1/refund-loss-audits/refund/{refundId}](refund-loss-audits/get-get-refund-loss-audit-by-refund-id-api-v1-refund-loss-audits-refund-refundid.md) — GET GetRefundLossAuditByRefundId
- [POST /api/v1/refund-loss-audits](refund-loss-audits/post-create-refund-loss-audit-api-v1-refund-loss-audits.md) — POST CreateRefundLossAudit
- [POST /api/v1/refund-loss-audits/approve](refund-loss-audits/post-approve-refund-loss-api-v1-refund-loss-audits-approve.md) — POST ApproveRefundLoss
- [DELETE /api/v1/refund-loss-audits/{id}](refund-loss-audits/delete-delete-refund-loss-audit-api-v1-refund-loss-audits-id.md) — DELETE DeleteRefundLossAudit

## Refunds

Manages payment refunds

- [GET /api/v1/refunds/{id}](refunds/get-get-refund-by-id-api-v1-refunds-id.md) — Manages payment refunds
- [POST /api/v1/refunds](refunds/post-create-refund-api-v1-refunds.md) — Retrieves all refunds for a specific invoice
- [POST /api/v1/refunds/{id}/process](refunds/post-process-refund-api-v1-refunds-id-process.md) — Processes a pending refund

## Registered Domain Histories

Provides read access to registered domain history entries.

- [GET /api/v1/registered-domain-histories/{id:int}](registered-domain-histories/get-get-by-id-api-v1-registered-domain-histories-id-int.md) — Provides read access to registered domain history entries.

## Registered Domains

Manages domains including creation, retrieval, updates, and deletion

- [GET /api/v1/registered-domains](registered-domains/get-get-all-domains-api-v1-registered-domains.md) — Manages domains including creation, retrieval, updates, and deletion
- [GET /api/v1/registered-domains/{id}](registered-domains/get-get-domain-by-id-api-v1-registered-domains-id.md) — Retrieves domains for a specific customer
- [GET /api/v1/registered-domains/name/{name}](registered-domains/get-get-domain-by-name-api-v1-registered-domains-name-name.md) — Retrieves a specific domain by name
- [POST /api/v1/registered-domains](registered-domains/post-create-domain-api-v1-registered-domains.md) — Creates a new domain
- [PUT /api/v1/registered-domains/{id}](registered-domains/put-update-domain-api-v1-registered-domains-id.md) — Updates an existing domain
- [DELETE /api/v1/registered-domains/{id}](registered-domains/delete-delete-domain-api-v1-registered-domains-id.md) — Deletes a domain
- [POST /api/v1/registered-domains/register](registered-domains/post-register-domain-api-v1-registered-domains-register.md) — Registers a new domain for the authenticated customer
- [POST /api/v1/registered-domains/register-for-customer](registered-domains/post-register-domain-for-customer-api-v1-registered-domains-register-for-customer.md) — Registers a new domain for a specific customer (Admin/Sales only)
- [POST /api/v1/registered-domains/check-availability](registered-domains/post-check-domain-availability-api-v1-registered-domains-check-availability.md) — Checks if a domain is available for registration
- [GET /api/v1/registered-domains/pricing/{tld}](registered-domains/get-get-domain-pricing-api-v1-registered-domains-pricing-tld.md) — Gets pricing information for a specific TLD

## Registrar Mail Addresses

Manages registrar mail address information including creation, retrieval, updates, and deletion

- [GET /api/v1/customers/{customerId}/registrar-mail-addresses](registrar-mail-addresses/get-get-registrar-mail-addresses-api-v1-customers-customerid-registrar-mail-addresses.md) — Manages registrar mail address information including creation, retrieval, updates, and deletion
- [GET /api/v1/customers/{customerId}/registrar-mail-addresses/default](registrar-mail-addresses/get-get-default-mail-address-api-v1-customers-customerid-registrar-mail-addresses-default.md) — Retrieves the default mail address for a specific customer
- [GET /api/v1/customers/{customerId}/registrar-mail-addresses/{id}](registrar-mail-addresses/get-get-registrar-mail-address-by-id-api-v1-customers-customerid-registrar-mail-addresses-id.md) — Retrieves a specific registrar mail address by its unique identifier
- [POST /api/v1/customers/{customerId}/registrar-mail-addresses](registrar-mail-addresses/post-create-registrar-mail-address-api-v1-customers-customerid-registrar-mail-addresses.md) — Creates a new registrar mail address
- [PUT /api/v1/customers/{customerId}/registrar-mail-addresses/{id}](registrar-mail-addresses/put-update-registrar-mail-address-api-v1-customers-customerid-registrar-mail-addresses-id.md) — Updates an existing registrar mail address
- [DELETE /api/v1/customers/{customerId}/registrar-mail-addresses/{id}](registrar-mail-addresses/delete-delete-registrar-mail-address-api-v1-customers-customerid-registrar-mail-addresses-id.md) — Deletes a registrar mail address
- [PUT /api/v1/customers/{customerId}/registrar-mail-addresses/{id}/set-default](registrar-mail-addresses/put-set-default-mail-address-api-v1-customers-customerid-registrar-mail-addresses-id-set-default.md) — Sets a registrar mail address as the default for the customer

## Registrars

Manages domain registrars and their TLD offerings

- [POST /api/v1/registrars/{registrarId}/tlds/download/{tld}](registrars/post-download-tlds-for-registrar-filtered-api-v1-registrars-registrarid-tlds-download-tld.md) — Manages domain registrars and their TLD offerings
- [POST /api/v1/registrars/{registrarId}/tlds/download/list](registrars/post-download-tlds-for-registrar-filtered-list-api-v1-registrars-registrarid-tlds-download-list.md) — Downloads TLDs for the registrar filtered by a list of TLD strings (provided in request body) and updates the database
- [POST /api/v1/registrars/{registrarId}/tlds/download](registrars/post-download-tlds-for-registrar-api-v1-registrars-registrarid-tlds-download.md) — Downloads all TLDs supported by the registrar from their API and updates the database
- [POST /api/v1/registrars/{registrarId}/tld/{tldId}](registrars/post-assign-tld-to-registrar-by-ids-api-v1-registrars-registrarid-tld-tldid.md) — Assigns a TLD to a registrar using their unique identifiers
- [POST /api/v1/registrars/{registrarId}/tld](registrars/post-assign-tld-to-registrar-by-dto-api-v1-registrars-registrarid-tld.md) — Assigns a TLD to a registrar using TLD details
- [GET /api/v1/registrars/{id}](registrars/get-get-registrar-by-id-api-v1-registrars-id.md) — Retrieves all domain registrars in the system
- [GET /api/v1/registrars/code/{code}](registrars/get-get-registrar-by-code-api-v1-registrars-code-code.md) — Get registrar by code
- [POST /api/v1/registrars](registrars/post-create-registrar-api-v1-registrars.md) — Create a new registrar
- [PUT /api/v1/registrars/{id}](registrars/put-update-registrar-api-v1-registrars-id.md) — Update an existing registrar
- [DELETE /api/v1/registrars/{id}](registrars/delete-delete-registrar-api-v1-registrars-id.md) — Delete a registrar
- [GET /api/v1/registrars/{registrarId}/isavailable/{domainName}](registrars/get-check-domain-availability-api-v1-registrars-registrarid-isavailable-domainname.md) — Checks if a domain is available for registration using a specific registrar
- [POST /api/v1/registrars/{registrarId}/domains/download](registrars/post-download-domains-for-registrar-api-v1-registrars-registrarid-domains-download.md) — Downloads all domains registered with a specific registrar and syncs them to the database
- [POST /api/v1/registrars/{registrarId}/domains/dns-records/sync](registrars/post-sync-dns-records-for-registrar-domains-api-v1-registrars-registrarid-domains-dns-records-sync.md) — Downloads DNS records from the registrar for all domains in the local database     and merges them into the DnsRecords table.
- [GET /api/v1/registrars/{registrarId}/domains](registrars/get-get-registered-domains-api-v1-registrars-registrarid-domains.md) — Gets all domains registered with a specific registrar
- [POST /api/v1/registrars/{id}/set-default](registrars/post-set-default-registrar-api-v1-registrars-id-set-default.md) — Gets all registrant contacts available in the registrar account.

## Registrar Tld Cost Pricing

Manages registrar TLD cost pricing (temporal pricing for registrar costs)

- [GET /api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}/current](registrar-tld-cost-pricing/get-get-current-cost-pricing-api-v1-registrar-tld-cost-pricing-registrar-tld-registrartldid-current.md) — Manages registrar TLD cost pricing (temporal pricing for registrar costs)
- [POST /api/v1/registrar-tld-cost-pricing](registrar-tld-cost-pricing/post-create-cost-pricing-api-v1-registrar-tld-cost-pricing.md) — Retrieves future scheduled cost pricing for a registrar-TLD
- [PUT /api/v1/registrar-tld-cost-pricing/{id}](registrar-tld-cost-pricing/put-update-cost-pricing-api-v1-registrar-tld-cost-pricing-id.md) — Updates existing cost pricing (only future pricing if configured)
- [DELETE /api/v1/registrar-tld-cost-pricing/{id}](registrar-tld-cost-pricing/delete-delete-future-cost-pricing-api-v1-registrar-tld-cost-pricing-id.md) — Deletes future cost pricing (only if not yet effective and configured to allow)

## Registrar Tlds

Manages registrar-TLD relationships and pricing

- [GET /api/v1/registrar-tlds](registrar-tlds/get-get-all-registrar-tlds-api-v1-registrar-tlds.md) — Manages registrar-TLD relationships and pricing
- [GET /api/v1/registrar-tlds/registrar/{registrarId}](registrar-tlds/get-get-registrar-tlds-by-registrar-api-v1-registrar-tlds-registrar-registrarid.md) — Retrieves only available registrar-TLD offerings for purchase
- [GET /api/v1/registrar-tlds/{id}](registrar-tlds/get-get-registrar-tld-by-id-api-v1-registrar-tlds-id.md) — Retrieves all registrars offering a specific TLD
- [GET /api/v1/registrar-tlds/registrar/{registrarId}/tld/{tldId}](registrar-tlds/get-get-registrar-tld-by-registrar-and-tld-api-v1-registrar-tlds-registrar-registrarid-tld-tldid.md) — Retrieves a specific registrar-TLD offering by registrar and TLD combination
- [POST /api/v1/registrar-tlds](registrar-tlds/post-create-registrar-tld-api-v1-registrar-tlds.md) — Creates a new registrar-TLD offering with pricing information
- [PUT /api/v1/registrar-tlds/{id}](registrar-tlds/put-update-registrar-tld-api-v1-registrar-tlds-id.md) — Updates an existing registrar-TLD offering with new pricing information
- [DELETE /api/v1/registrar-tlds/{id}](registrar-tlds/delete-delete-registrar-tld-api-v1-registrar-tlds-id.md) — Delete a registrar TLD offering
- [POST /api/v1/registrar-tlds/registrar/{registrarId}/import](registrar-tlds/post-import-registrar-tlds-api-v1-registrar-tlds-registrar-registrarid-import.md) — Imports TLDs for a specific registrar from CSV format form data
- [POST /api/v1/registrar-tlds/registrar/{registrarId}/upload-csv](registrar-tlds/post-upload-registrar-tlds-csv-api-v1-registrar-tlds-registrar-registrarid-upload-csv.md) — Uploads a CSV file with TLDs to merge into the Tlds table and add references in RegistrarTlds table
- [PUT /api/v1/registrar-tlds/bulk-update-status](registrar-tlds/put-bulk-update-all-registrar-tld-status-api-v1-registrar-tlds-bulk-update-status.md) — Updates the active status of all registrar-TLD offerings in the system
- [PUT /api/v1/registrar-tlds/bulk-update-status-by-tld](registrar-tlds/put-bulk-update-registrar-tld-status-by-tld-api-v1-registrar-tlds-bulk-update-status-by-tld.md) — Updates the active status of registrar-TLD offerings for specific TLD extensions

## Report Templates

Manages report templates for FastReport and other reporting engines

- [GET /api/v1/report-templates/{id}](report-templates/get-get-template-by-id-api-v1-report-templates-id.md) — Manages report templates for FastReport and other reporting engines
- [GET /api/v1/report-templates/default/{type}](report-templates/get-get-default-template-api-v1-report-templates-default-type.md) — Retrieves the default template for a specific type
- [POST /api/v1/report-templates](report-templates/post-create-template-api-v1-report-templates.md) — Searches for report templates by name, description, or tags
- [PUT /api/v1/report-templates/{id}](report-templates/put-update-template-api-v1-report-templates-id.md) — Updates an existing report template
- [PUT /api/v1/report-templates/{id}/set-default](report-templates/put-set-default-template-api-v1-report-templates-id-set-default.md) — Sets a template as the default for its type
- [PUT /api/v1/report-templates/{id}/toggle-active](report-templates/put-toggle-active-status-api-v1-report-templates-id-toggle-active.md) — Toggles the active status of a report template
- [GET /api/v1/report-templates/{id}/download](report-templates/get-download-template-api-v1-report-templates-id-download.md) — Downloads the template file
- [DELETE /api/v1/report-templates/{id}](report-templates/delete-delete-template-api-v1-report-templates-id.md) — Soft deletes a report template

## Reseller Companies

Manages reseller companies including creation, retrieval, updates, and deletion

- [GET /api/v1/reseller-companies/default](reseller-companies/get-get-default-reseller-company-api-v1-reseller-companies-default.md) — Manages reseller companies including creation, retrieval, updates, and deletion
- [GET /api/v1/reseller-companies/{id}](reseller-companies/get-get-reseller-company-by-id-api-v1-reseller-companies-id.md) — Retrieves a specific reseller company by ID
- [POST /api/v1/reseller-companies](reseller-companies/post-create-reseller-company-api-v1-reseller-companies.md) — Creates a new reseller company
- [PUT /api/v1/reseller-companies/{id}](reseller-companies/put-update-reseller-company-api-v1-reseller-companies-id.md) — Updates an existing reseller company
- [DELETE /api/v1/reseller-companies/{id}](reseller-companies/delete-delete-reseller-company-api-v1-reseller-companies-id.md) — Deletes a reseller company

## Roles

Manages user roles including creation, retrieval, updates, and deletion

- [GET /api/v1/roles/{id}](roles/get-get-role-by-id-api-v1-roles-id.md) — Manages user roles including creation, retrieval, updates, and deletion
- [POST /api/v1/roles](roles/post-create-role-api-v1-roles.md) — Creates a new role in the system
- [PUT /api/v1/roles/{id}](roles/put-update-role-api-v1-roles-id.md) — Updates an existing role's information
- [DELETE /api/v1/roles/{id}](roles/delete-delete-role-api-v1-roles-id.md) — Deletes a role from the system

## Sales Agents

Manages sales agents including creation, retrieval, updates, and deletion

- [GET /api/v1/sales-agents/{id}](sales-agents/get-get-sales-agent-by-id-api-v1-sales-agents-id.md) — Manages sales agents including creation, retrieval, updates, and deletion
- [POST /api/v1/sales-agents](sales-agents/post-create-sales-agent-api-v1-sales-agents.md) — Creates a new sales agent
- [PUT /api/v1/sales-agents/{id}](sales-agents/put-update-sales-agent-api-v1-sales-agents-id.md) — Updates an existing sales agent
- [DELETE /api/v1/sales-agents/{id}](sales-agents/delete-delete-sales-agent-api-v1-sales-agents-id.md) — Deletes a sales agent

## Sent Emails

Manages sent email records including creation, retrieval, updates, and deletion

- [GET /api/v1/sent-emails/by-message-id/{messageId}](sent-emails/get-get-sent-email-by-message-id-api-v1-sent-emails-by-message-id-messageid.md) — Manages sent email records including creation, retrieval, updates, and deletion
- [GET /api/v1/sent-emails/{id}](sent-emails/get-get-sent-email-by-id-api-v1-sent-emails-id.md) — Retrieves a specific sent email by ID
- [POST /api/v1/sent-emails](sent-emails/post-create-sent-email-api-v1-sent-emails.md) — Creates a new sent email record
- [PUT /api/v1/sent-emails/{id}](sent-emails/put-update-sent-email-api-v1-sent-emails-id.md) — Updates an existing sent email record
- [DELETE /api/v1/sent-emails/{id}](sent-emails/delete-delete-sent-email-api-v1-sent-emails-id.md) — Deletes a sent email record

## Server Control Panels

Manages server control panels including creation, retrieval, updates, deletion, and connection testing

- [GET /api/v1/server-control-panels/{id}](server-control-panels/get-get-server-control-panel-by-id-api-v1-server-control-panels-id.md) — Manages server control panels including creation, retrieval, updates, deletion, and connection testing
- [POST /api/v1/server-control-panels](server-control-panels/post-create-server-control-panel-api-v1-server-control-panels.md) — Creates a new server control panel in the system
- [PUT /api/v1/server-control-panels/{id}](server-control-panels/put-update-server-control-panel-api-v1-server-control-panels-id.md) — Updates an existing server control panel's information
- [DELETE /api/v1/server-control-panels/{id}](server-control-panels/delete-delete-server-control-panel-api-v1-server-control-panels-id.md) — Deletes a server control panel from the system
- [POST /api/v1/server-control-panels/{id}/test-connection](server-control-panels/post-test-connection-api-v1-server-control-panels-id-test-connection.md) — Tests the connection to a server control panel

## Server Ip Addresses

Manages server IP addresses including creation, retrieval, updates, and deletion

- [GET /api/v1/server-ip-addresses/{id}](server-ip-addresses/get-get-server-ip-address-by-id-api-v1-server-ip-addresses-id.md) — Manages server IP addresses including creation, retrieval, updates, and deletion
- [POST /api/v1/server-ip-addresses](server-ip-addresses/post-create-server-ip-address-api-v1-server-ip-addresses.md) — Creates a new server IP address in the system
- [PUT /api/v1/server-ip-addresses/{id}](server-ip-addresses/put-update-server-ip-address-api-v1-server-ip-addresses-id.md) — Updates an existing server IP address's information
- [DELETE /api/v1/server-ip-addresses/{id}](server-ip-addresses/delete-delete-server-ip-address-api-v1-server-ip-addresses-id.md) — Deletes a server IP address from the system

## Servers

Manages servers including creation, retrieval, updates, and deletion

- [GET /api/v1/servers/{id}](servers/get-get-server-by-id-api-v1-servers-id.md) — Manages servers including creation, retrieval, updates, and deletion
- [POST /api/v1/servers](servers/post-create-server-api-v1-servers.md) — Creates a new server in the system
- [PUT /api/v1/servers/{id}](servers/put-update-server-api-v1-servers-id.md) — Updates an existing server's information
- [DELETE /api/v1/servers/{id}](servers/delete-delete-server-api-v1-servers-id.md) — Deletes a server from the system

## Server Types

Manages server types including creation, retrieval, updates, and deletion

- [GET /api/v1/server-types/{id}](server-types/get-get-server-type-by-id-api-v1-server-types-id.md) — Manages server types including creation, retrieval, updates, and deletion
- [POST /api/v1/server-types](server-types/post-create-server-type-api-v1-server-types.md) — Creates a new server type in the system
- [PUT /api/v1/server-types/{id}](server-types/put-update-server-type-api-v1-server-types-id.md) — Updates an existing server type's information
- [DELETE /api/v1/server-types/{id}](server-types/delete-delete-server-type-api-v1-server-types-id.md) — Deletes a server type from the system

## Services

Manages services offered to customers including creation, retrieval, updates, and deletion

- [GET /api/v1/services/{id}](services/get-get-service-by-id-api-v1-services-id.md) — Manages services offered to customers including creation, retrieval, updates, and deletion
- [POST /api/v1/services](services/post-create-service-api-v1-services.md) — Creates a new service in the system
- [PUT /api/v1/services/{id}](services/put-update-service-api-v1-services-id.md) — Updates an existing service's information
- [DELETE /api/v1/services/{id}](services/delete-delete-service-api-v1-services-id.md) — Deletes a service from the system

## Service Types

Manages service types including creation, retrieval, updates, and deletion

- [GET /api/v1/service-types/{id}](service-types/get-get-service-type-by-id-api-v1-service-types-id.md) — Manages service types including creation, retrieval, updates, and deletion
- [GET /api/v1/service-types/by-name/{name}](service-types/get-get-service-type-by-name-api-v1-service-types-by-name-name.md) — Retrieves a specific service type by its name
- [POST /api/v1/service-types](service-types/post-create-service-type-api-v1-service-types.md) — Creates a new service type in the system
- [PUT /api/v1/service-types/{id}](service-types/put-update-service-type-api-v1-service-types-id.md) — Updates an existing service type's information
- [DELETE /api/v1/service-types/{id}](service-types/delete-delete-service-type-api-v1-service-types-id.md) — Deletes a service type from the system

## Sold Hosting Packages

API endpoints for managing SoldHostingPackages resources.

- [GET /api/v1/sold-hosting-packages/{id:int}](sold-hosting-packages/get-get-by-id-api-v1-sold-hosting-packages-id-int.md) — GET GetById
- [POST /api/v1/sold-hosting-packages](sold-hosting-packages/post-create-api-v1-sold-hosting-packages.md) — POST Create
- [PUT /api/v1/sold-hosting-packages/{id:int}](sold-hosting-packages/put-update-api-v1-sold-hosting-packages-id-int.md) — PUT Update
- [DELETE /api/v1/sold-hosting-packages/{id:int}](sold-hosting-packages/delete-delete-api-v1-sold-hosting-packages-id-int.md) — DELETE Delete

## Sold Optional Services

API endpoints for managing SoldOptionalServices resources.

- [GET /api/v1/sold-optional-services/{id:int}](sold-optional-services/get-get-by-id-api-v1-sold-optional-services-id-int.md) — GET GetById
- [POST /api/v1/sold-optional-services](sold-optional-services/post-create-api-v1-sold-optional-services.md) — POST Create
- [PUT /api/v1/sold-optional-services/{id:int}](sold-optional-services/put-update-api-v1-sold-optional-services-id-int.md) — PUT Update
- [DELETE /api/v1/sold-optional-services/{id:int}](sold-optional-services/delete-delete-api-v1-sold-optional-services-id-int.md) — DELETE Delete

## Subscription Billing Histories

Manages subscription billing history records for audit and tracking purposes

- [GET /api/v1/subscription-billing-histories/{id}](subscription-billing-histories/get-get-billing-history-by-id-api-v1-subscription-billing-histories-id.md) — Manages subscription billing history records for audit and tracking purposes
- [POST /api/v1/subscription-billing-histories](subscription-billing-histories/post-create-billing-history-api-v1-subscription-billing-histories.md) — Creates a new billing history record (typically used for manual entries)
- [DELETE /api/v1/subscription-billing-histories/{id}](subscription-billing-histories/delete-delete-billing-history-api-v1-subscription-billing-histories-id.md) — Deletes a billing history record

## Subscriptions

Manages recurring billing subscriptions including creation, updates, cancellation, and billing operations

- [GET /api/v1/subscriptions/{id}](subscriptions/get-get-subscription-by-id-api-v1-subscriptions-id.md) — Manages recurring billing subscriptions including creation, updates, cancellation, and billing operations
- [POST /api/v1/subscriptions](subscriptions/post-create-subscription-api-v1-subscriptions.md) — Creates a new subscription
- [PUT /api/v1/subscriptions/{id}](subscriptions/put-update-subscription-api-v1-subscriptions-id.md) — Updates an existing subscription
- [POST /api/v1/subscriptions/{id}/cancel](subscriptions/post-cancel-subscription-api-v1-subscriptions-id-cancel.md) — Cancels a subscription
- [POST /api/v1/subscriptions/{id}/pause](subscriptions/post-pause-subscription-api-v1-subscriptions-id-pause.md) — Pauses a subscription
- [POST /api/v1/subscriptions/{id}/resume](subscriptions/post-resume-subscription-api-v1-subscriptions-id-resume.md) — Resumes a paused subscription
- [DELETE /api/v1/subscriptions/{id}](subscriptions/delete-delete-subscription-api-v1-subscriptions-id.md) — Deletes a subscription
- [POST /api/v1/subscriptions/{id}/process-billing](subscriptions/post-process-subscription-billing-api-v1-subscriptions-id-process-billing.md) — Manually processes billing for a specific subscription
- [POST /api/v1/subscriptions/check-statuses](subscriptions/post-check-statuses-api-v1-subscriptions-check-statuses.md) — Manually runs subscription status checks against configured payment gateways.

## Support Tickets

Provides support ticket endpoints for customers and support staff.

- [GET /api/v1/support-tickets](support-tickets/get-get-tickets-api-v1-support-tickets.md) — Provides support ticket endpoints for customers and support staff.
- [GET /api/v1/support-tickets/{id}](support-tickets/get-get-by-id-api-v1-support-tickets-id.md) — Retrieves one support ticket by identifier.
- [POST /api/v1/support-tickets](support-tickets/post-create-api-v1-support-tickets.md) — Creates a new support ticket for the authenticated customer.
- [POST /api/v1/support-tickets/{id}/messages](support-tickets/post-add-message-api-v1-support-tickets-id-messages.md) — Adds a message to a support ticket conversation.
- [PATCH /api/v1/support-tickets/{id}/status](support-tickets/patch-update-status-api-v1-support-tickets-id-status.md) — Updates the status and assignee of a support ticket.

## System

System-level operations including data normalization and maintenance tasks

- [POST /api/v1/system/accept-offer](system/post-accept-offer-api-v1-system-accept-offer.md) — System-level operations including data normalization and maintenance tasks
- [POST /api/v1/system/verify-offer-print](system/post-verify-offer-print-api-v1-system-verify-offer-print.md) — Generates an offer PDF on the server for print verification
- [GET /api/v1/system/sales-summary](system/get-get-sales-summary-api-v1-system-sales-summary.md) — Retrieves dashboard sales summary with offers, orders and open invoices
- [GET /api/v1/system/offer-editor/{quoteId:int}](system/get-get-offer-editor-snapshot-api-v1-system-offer-editor-quoteid-int.md) — Retrieves the latest persisted offer snapshot for a quote so it can be reopened in the offer editor
- [POST /api/v1/system/send-offer](system/post-send-offer-api-v1-system-send-offer.md) — Persists and marks an offer as sent while generating a server-side PDF snapshot
- [POST /api/v1/system/normalize-all-records](system/post-normalize-all-records-api-v1-system-normalize-all-records.md) — Normalizes all records in the database by updating normalized fields for exact searches
- [GET /api/v1/system/health](system/get-health-api-v1-system-health.md) — Health check endpoint for the system controller
- [POST /api/v1/system/backup](system/post-create-backup-api-v1-system-backup.md) — Creates a backup of the database
- [POST /api/v1/system/restore](system/post-restore-from-backup-api-v1-system-restore.md) — Restores the database from a backup file

## System Settings

Manages system settings including creation, retrieval, updates, and deletion

- [GET /api/v1/system-settings/{id:int}](system-settings/get-get-system-setting-by-id-api-v1-system-settings-id-int.md) — Manages system settings including creation, retrieval, updates, and deletion
- [GET /api/v1/system-settings/key/{key}](system-settings/get-get-system-setting-by-key-api-v1-system-settings-key-key.md) — Retrieves a specific system setting by its key
- [POST /api/v1/system-settings](system-settings/post-create-system-setting-api-v1-system-settings.md) — Creates a new system setting
- [PUT /api/v1/system-settings/{id}](system-settings/put-update-system-setting-api-v1-system-settings-id.md) — Updates an existing system setting
- [DELETE /api/v1/system-settings/{id}](system-settings/delete-delete-system-setting-api-v1-system-settings-id.md) — Deletes a system setting

## Tax Calculation

Provides quote and finalize tax endpoints for checkout and invoicing.

- [POST /api/v1/tax/quote](tax-calculation/post-quote-api-v1-tax-quote.md) — Provides quote and finalize tax endpoints for checkout and invoicing.
- [POST /api/v1/tax/finalize](tax-calculation/post-finalize-api-v1-tax-finalize.md) — Finalizes tax calculation and persists an immutable order tax snapshot.

## Tax Categories

Manages tax categories by country and optional state.

- [GET /api/v1/tax-categories/{id}](tax-categories/get-get-tax-category-by-id-api-v1-tax-categories-id.md) — Manages tax categories by country and optional state.
- [POST /api/v1/tax-categories](tax-categories/post-create-tax-category-api-v1-tax-categories.md) — Creates a tax category.
- [PUT /api/v1/tax-categories/{id}](tax-categories/put-update-tax-category-api-v1-tax-categories-id.md) — Updates a tax category.
- [DELETE /api/v1/tax-categories/{id}](tax-categories/delete-delete-tax-category-api-v1-tax-categories-id.md) — Deletes a tax category.

## Tax Jurisdictions

Manages tax jurisdictions used by VAT and TAX calculation.

- [GET /api/v1/tax-jurisdictions/{id}](tax-jurisdictions/get-get-tax-jurisdiction-by-id-api-v1-tax-jurisdictions-id.md) — Manages tax jurisdictions used by VAT and TAX calculation.
- [POST /api/v1/tax-jurisdictions](tax-jurisdictions/post-create-tax-jurisdiction-api-v1-tax-jurisdictions.md) — Creates a tax jurisdiction.
- [PUT /api/v1/tax-jurisdictions/{id}](tax-jurisdictions/put-update-tax-jurisdiction-api-v1-tax-jurisdictions-id.md) — Updates a tax jurisdiction.
- [DELETE /api/v1/tax-jurisdictions/{id}](tax-jurisdictions/delete-delete-tax-jurisdiction-api-v1-tax-jurisdictions-id.md) — Deletes a tax jurisdiction.

## Tax Registrations

Manages seller tax registrations used by VAT and TAX reporting.

- [GET /api/v1/tax-registrations/{id}](tax-registrations/get-get-tax-registration-by-id-api-v1-tax-registrations-id.md) — Manages seller tax registrations used by VAT and TAX reporting.
- [POST /api/v1/tax-registrations](tax-registrations/post-create-tax-registration-api-v1-tax-registrations.md) — Creates a tax registration.
- [PUT /api/v1/tax-registrations/{id}](tax-registrations/put-update-tax-registration-api-v1-tax-registrations-id.md) — Updates a tax registration.
- [DELETE /api/v1/tax-registrations/{id}](tax-registrations/delete-delete-tax-registration-api-v1-tax-registrations-id.md) — Deletes a tax registration.

## Tax Rules

Manages tax rules for automatic tax calculation

- [GET /api/v1/tax-rules/{id}](tax-rules/get-get-tax-rule-by-id-api-v1-tax-rules-id.md) — Manages tax rules for automatic tax calculation
- [POST /api/v1/tax-rules](tax-rules/post-create-tax-rule-api-v1-tax-rules.md) — Retrieves tax rules by location
- [PUT /api/v1/tax-rules/{id}](tax-rules/put-update-tax-rule-api-v1-tax-rules-id.md) — Updates an existing tax rule
- [DELETE /api/v1/tax-rules/{id}](tax-rules/delete-delete-tax-rule-api-v1-tax-rules-id.md) — Deletes a tax rule (soft delete)
- [GET /api/v1/tax-rules/calculate](tax-rules/get-calculate-tax-api-v1-tax-rules-calculate.md) — Calculates tax for a customer and amount
- [POST /api/v1/tax-rules/validate-vat](tax-rules/post-validate-vat-number-api-v1-tax-rules-validate-vat.md) — Validates a VAT number (EU VIES check)

## Test

Provides admin-only testing endpoints.

- [POST /api/v1/test/email](test/post-send-test-email-api-v1-test-email.md) — Provides admin-only testing endpoints.
- [POST /api/v1/test/seed-data](test/post-seed-test-data-api-v1-test-seed-data.md) — Seeds test catalog data into selected tables when those tables are empty.
- [POST /api/v1/test/admin-mycompany/export](test/post-export-admin-my-company-api-v1-test-admin-mycompany-export.md) — Exports the current admin user and MyCompany profile to a debug snapshot file.
- [POST /api/v1/test/admin-mycompany/import](test/post-import-admin-my-company-api-v1-test-admin-mycompany-import.md) — Imports admin user and MyCompany profile from a debug snapshot file.
- [GET /api/v1/test/build-mode](test/get-get-build-mode-api-v1-test-build-mode.md) — Returns whether the API is running as a Debug or Release build.
- [GET /api/v1/test/debug-runtime-info](test/get-get-debug-runtime-info-api-v1-test-debug-runtime-info.md) — Returns debug-only runtime details used by the reseller debug help page.

## Tld Pricing

Manages TLD sales pricing (temporal pricing for customer-facing prices)

- [GET /api/v1/tld-pricing/sales/tld/{tldId}/current](tld-pricing/get-get-current-sales-pricing-api-v1-tld-pricing-sales-tld-tldid-current.md) — Manages TLD sales pricing (temporal pricing for customer-facing prices)
- [POST /api/v1/tld-pricing/sales](tld-pricing/post-create-sales-pricing-api-v1-tld-pricing-sales.md) — Creates new sales pricing for a TLD
- [PUT /api/v1/tld-pricing/sales/{id}](tld-pricing/put-update-sales-pricing-api-v1-tld-pricing-sales-id.md) — Updates existing sales pricing (only future pricing if configured)
- [DELETE /api/v1/tld-pricing/sales/{id}](tld-pricing/delete-delete-future-sales-pricing-api-v1-tld-pricing-sales-id.md) — Deletes future sales pricing
- [POST /api/v1/tld-pricing/calculate](tld-pricing/post-calculate-pricing-api-v1-tld-pricing-calculate.md) — Calculates final pricing for a TLD including discounts and promotions
- [GET /api/v1/tld-pricing/margin/tld/{tldId}](tld-pricing/get-calculate-margin-api-v1-tld-pricing-margin-tld-tldid.md) — Calculates margin for a specific TLD and operation type
- [POST /api/v1/tld-pricing/archive](tld-pricing/post-archive-old-pricing-api-v1-tld-pricing-archive.md) — Gets all TLDs with negative margins (cost > price)

## Tld Registry Rules

Manages TLD registry policy rules.

- [GET /api/v1/tld-registry-rules/{id:int}](tld-registry-rules/get-get-by-id-api-v1-tld-registry-rules-id-int.md) — Manages TLD registry policy rules.
- [POST /api/v1/tld-registry-rules](tld-registry-rules/post-create-api-v1-tld-registry-rules.md) — Creates a new TLD registry rule.
- [PUT /api/v1/tld-registry-rules/{id:int}](tld-registry-rules/put-update-api-v1-tld-registry-rules-id-int.md) — Updates an existing TLD registry rule.
- [DELETE /api/v1/tld-registry-rules/{id:int}](tld-registry-rules/delete-delete-api-v1-tld-registry-rules-id-int.md) — Deletes a TLD registry rule.

## Tlds

Manages Top-Level Domains (TLDs) and their registrars

- [GET /api/v1/tlds](tlds/get-get-all-tlds-api-v1-tlds.md) — Manages Top-Level Domains (TLDs) and their registrars
- [GET /api/v1/tlds/active](tlds/get-get-active-tlds-api-v1-tlds-active.md) — Retrieves only active Top-Level Domains
- [GET /api/v1/tlds/secondlevel](tlds/get-get-second-level-tlds-api-v1-tlds-secondlevel.md) — Retrieves only second-level Top-Level Domains
- [GET /api/v1/tlds/toplevel](tlds/get-get-top-level-tlds-api-v1-tlds-toplevel.md) — Retrieves only top-level (non-second-level) Top-Level Domains
- [GET /api/v1/tlds/{id}](tlds/get-get-tld-by-id-api-v1-tlds-id.md) — Retrieves a specific TLD by its unique identifier
- [GET /api/v1/tlds/extension/{extension}](tlds/get-get-tld-by-extension-api-v1-tlds-extension-extension.md) — Retrieves a specific TLD by its extension
- [POST /api/v1/tlds](tlds/post-create-tld-api-v1-tlds.md) — Create a new TLD
- [PUT /api/v1/tlds/{id}](tlds/put-update-tld-api-v1-tlds-id.md) — Update an existing TLD
- [DELETE /api/v1/tlds/{id}](tlds/delete-delete-tld-api-v1-tlds-id.md) — Delete a TLD

## Tokens

Manages authentication and refresh tokens

- [GET /api/v1/tokens/{id}](tokens/get-get-token-by-id-api-v1-tokens-id.md) — Manages authentication and refresh tokens
- [POST /api/v1/tokens](tokens/post-create-token-api-v1-tokens.md) — Creates a new token (Admin only)
- [PUT /api/v1/tokens/{id}](tokens/put-update-token-api-v1-tokens-id.md) — Updates an existing token's information
- [DELETE /api/v1/tokens/{id}](tokens/delete-delete-token-api-v1-tokens-id.md) — Delete a token

## Units

Manages units of measurement for services and products

- [GET /api/v1/units/{id}](units/get-get-unit-by-id-api-v1-units-id.md) — Manages units of measurement for services and products
- [GET /api/v1/units/code/{code}](units/get-get-unit-by-code-api-v1-units-code-code.md) — Retrieves a specific unit by its code
- [POST /api/v1/units](units/post-create-unit-api-v1-units.md) — Creates a new unit of measurement in the system
- [PUT /api/v1/units/{id}](units/put-update-unit-api-v1-units-id.md) — Update an existing unit
- [DELETE /api/v1/units/{id}](units/delete-delete-unit-api-v1-units-id.md) — Delete a unit (soft delete)

## Users

Manages user accounts including creation, retrieval, updates, and deletion

- [GET /api/v1/users](users/get-get-all-users-api-v1-users.md) — Manages user accounts including creation, retrieval, updates, and deletion
- [GET /api/v1/users/{id}](users/get-get-user-by-id-api-v1-users-id.md) — Retrieves a specific user by their unique identifier
- [POST /api/v1/users](users/post-create-user-api-v1-users.md) — Creates a new user in the system
- [PUT /api/v1/users/{id}](users/put-update-user-api-v1-users-id.md) — Updates an existing user's information
- [DELETE /api/v1/users/{id}](users/delete-delete-user-api-v1-users-id.md) — Deletes a user from the system

## Vendor Costs

Manages vendor costs

- [GET /api/v1/vendor-costs/{id}](vendor-costs/get-get-vendor-cost-by-id-api-v1-vendor-costs-id.md) — Manages vendor costs
- [GET /api/v1/vendor-costs/summary/invoice/{invoiceId}](vendor-costs/get-get-vendor-cost-summary-by-invoice-id-api-v1-vendor-costs-summary-invoice-invoiceid.md) — Retrieves vendor costs by invoice line ID
- [POST /api/v1/vendor-costs](vendor-costs/post-create-vendor-cost-api-v1-vendor-costs.md) — Creates a new vendor cost
- [PUT /api/v1/vendor-costs/{id}](vendor-costs/put-update-vendor-cost-api-v1-vendor-costs-id.md) — Updates an existing vendor cost
- [DELETE /api/v1/vendor-costs/{id}](vendor-costs/delete-delete-vendor-cost-api-v1-vendor-costs-id.md) — Deletes a vendor cost

## Vendor Payouts

Manages vendor payouts

- [GET /api/v1/vendor-payouts/{id}](vendor-payouts/get-get-vendor-payout-by-id-api-v1-vendor-payouts-id.md) — Manages vendor payouts
- [GET /api/v1/vendor-payouts/summary/vendor/{vendorId}](vendor-payouts/get-get-vendor-payout-summary-by-vendor-id-api-v1-vendor-payouts-summary-vendor-vendorid.md) — GET GetVendorPayoutSummaryByVendorId
- [POST /api/v1/vendor-payouts](vendor-payouts/post-create-vendor-payout-api-v1-vendor-payouts.md) — POST CreateVendorPayout
- [PUT /api/v1/vendor-payouts/{id}](vendor-payouts/put-update-vendor-payout-api-v1-vendor-payouts-id.md) — PUT UpdateVendorPayout
- [DELETE /api/v1/vendor-payouts/{id}](vendor-payouts/delete-delete-vendor-payout-api-v1-vendor-payouts-id.md) — DELETE DeleteVendorPayout
- [POST /api/v1/vendor-payouts/process](vendor-payouts/post-process-vendor-payout-api-v1-vendor-payouts-process.md) — POST ProcessVendorPayout
- [POST /api/v1/vendor-payouts/resolve-intervention](vendor-payouts/post-resolve-payout-intervention-api-v1-vendor-payouts-resolve-intervention.md) — POST ResolvePayoutIntervention

## Vendor Tax Profiles

Manages vendor tax profiles

- [GET /api/v1/vendor-tax-profiles/{id}](vendor-tax-profiles/get-get-vendor-tax-profile-by-id-api-v1-vendor-tax-profiles-id.md) — Manages vendor tax profiles
- [GET /api/v1/vendor-tax-profiles/vendor/{vendorId}](vendor-tax-profiles/get-get-vendor-tax-profile-by-vendor-id-api-v1-vendor-tax-profiles-vendor-vendorid.md) — GET GetVendorTaxProfileByVendorId
- [POST /api/v1/vendor-tax-profiles](vendor-tax-profiles/post-create-vendor-tax-profile-api-v1-vendor-tax-profiles.md) — POST CreateVendorTaxProfile
- [PUT /api/v1/vendor-tax-profiles/{id}](vendor-tax-profiles/put-update-vendor-tax-profile-api-v1-vendor-tax-profiles-id.md) — PUT UpdateVendorTaxProfile
- [DELETE /api/v1/vendor-tax-profiles/{id}](vendor-tax-profiles/delete-delete-vendor-tax-profile-api-v1-vendor-tax-profiles-id.md) — DELETE DeleteVendorTaxProfile

## DTOs

- [DTO Reference Index](dtos/index.md) — Data Transfer Objects used by API endpoints, including TypeScript interfaces.
