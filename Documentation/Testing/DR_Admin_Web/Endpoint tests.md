Endpoint tests using DR_Admin_Web

| Controller                               | Status | Comments                                                                                                                |
| ---------------------------------------- | ------ | ----------------------------------------------------------------------------------------------------------------------- |
| `AuthController`                         |        | No test page                                                                                                            |
| `BillingCyclesController`                | ISSUES | - Duration and Sort order not set                                                                                       |
| `ControlPanelTypesController`            |        |                                                                                                                         |
| `CountriesController`                    | OK     |                                                                                                                         |
| `CouponsController`                      | ISSUES | - Cannot create coupon                                                                                                  |
| `CurrenciesController`                   | ISSUES | - Cannot create Currency                                                                                                |
| `CustomerCreditsController`              | ISSUES | - Cannot register                                                                                                       |
| `CustomerPaymentMethodsController`       | ISSUES | - Test page not implemented?                                                                                            |
| `CustomersController`                    | ISSUES | - Connect customer to PostalCodes<br>- Extend with different addresses<br>- Country picker<br>- Set isCompany           |
| `DnsRecordsController`                   |        |                                                                                                                         |
| `DnsRecordTypesController`               |        |                                                                                                                         |
| `DnsZonePackageRecordsController`        |        |                                                                                                                         |
| `DnsZonePackagesController`              |        |                                                                                                                         |
| `DocumentTemplatesController`            | ISSUE  | - Limited type list<br>- Error on insert: Failed to fetch                                                               |
| `EmailQueueController`                   | ISSUE  | - Both html body and text body should be suppported.                                                                    |
| `HostingPackagesController`              |        |                                                                                                                         |
| `InitializationController`               | ISSUE  | - Must be redesigned                                                                                                    |
| `InvoiceLinesController`                 |        |                                                                                                                         |
| `InvoicesController`                     |        |                                                                                                                         |
| `MyAccountController`                    | ISSUE  | - Cannot register new user<br>- Endpoints not tested.<br>- Test page might be redesigned. Focus should be on MyAccount. |
| `OrdersController`                       |        |                                                                                                                         |
| `PaymentGatewaysController`              |        |                                                                                                                         |
| `PaymentIntentsController`               |        |                                                                                                                         |
| `PostalCodesController`                  | ISSUES | - Get Postal Codes by City: Case sensitivity                                                                            |
| `QuotesController`                       |        |                                                                                                                         |
| `RefundsController`                      |        |                                                                                                                         |
| `RegistrarsController`                   |        |                                                                                                                         |
| `RegistrarTldsController`                |        |                                                                                                                         |
| `ResellerCompaniesController`            | ISSUE  | - Not possible to register company                                                                                      |
| `RolesController`                        | ISSUE  | - Cannot register rolesD                                                                                                |
| `SalesAgentsController`                  | ISSUE  | - Should be related to a user                                                                                           |
| `SentEmailsController`                   | ISSUE  | - Test when i'ts possible to send mails                                                                                 |
| `ServerControlPanelsController`          |        |                                                                                                                         |
| `ServerIpAddressesController`            |        |                                                                                                                         |
| `ServersController`                      | ISSuE  | - Cannot register servers                                                                                               |
| `ServicesController`                     |        |                                                                                                                         |
| `ServiceTypesController`                 |        |                                                                                                                         |
| `SubscriptionBillingHistoriesController` |        |                                                                                                                         |
| `SubscriptionsController`                |        |                                                                                                                         |
| `SystemController`                       |        |                                                                                                                         |
| `TaxRulesController`                     | ISSUES | - Not all endpoints are included.                                                                                       |
| `TldsController`                         |        |                                                                                                                         |
| `TokensController`                       |        |                                                                                                                         |
| `UnitsController`                        | ISSUES | - Not all endpoints are included.                                                                                       |
| `UsersController`                        | ISSUES | - Get all users presents json only.<br>- Not all endpoints are included.<br>- Properties missing from POST              |
