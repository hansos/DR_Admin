# PagedResult

Generic wrapper for paginated results

## Source

`DR_Admin/DTOs/PagedResult.cs`

## TypeScript Interface

```ts
export interface PagedResult {
  data: T[];
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Data` | `List<T>` | `T[]` |
| `CurrentPage` | `int` | `number` |
| `PageSize` | `int` | `number` |
| `TotalCount` | `int` | `number` |
| `TotalPages` | `int` | `number` |

## Used By Endpoints

- [GET GetAllAddressTypes](../address-types/get-get-all-address-types-api-v1-address-types.md)
- [GET GetAllContactPersons](../contact-persons/get-get-all-contact-persons-api-v1-contact-persons.md)
- [GET GetActiveCountries](../countries/get-get-active-countries-api-v1-countries-active.md)
- [GET GetAllCountries](../countries/get-get-all-countries-api-v1-countries.md)
- [GET GetAllCustomers](../customers/get-get-all-customers-api-v1-customers.md)
- [GET GetAllDnsRecords](../dns-records/get-get-all-dns-records-api-v1-dns-records.md)
- [GET GetAllDomainContacts](../domain-contacts/get-get-all-domain-contacts-api-v1-domain-contacts.md)
- [GET GetAllInvoiceLines](../invoice-lines/get-get-all-invoice-lines-api-v1-invoice-lines.md)
- [GET GetAllInvoices](../invoices/get-get-all-invoices-api-v1-invoices.md)
- [GET GetInvoicesByCustomerId](../invoices/get-get-invoices-by-customer-id-api-v1-invoices-customer-customerid.md)
- [GET GetAll](../login-histories/get-get-all-api-v1-login-histories.md)
- [GET GetAllNameServers](../name-servers/get-get-all-name-servers-api-v1-name-servers.md)
- [GET GetAllPostalCodes](../postal-codes/get-get-all-postal-codes-api-v1-postal-codes.md)
- [GET GetAllDomains](../registered-domains/get-get-all-domains-api-v1-registered-domains.md)
- [GET GetAllRegistrarTlds](../registrar-tlds/get-get-all-registrar-tlds-api-v1-registrar-tlds.md)
- [GET GetRegistrarTldsByRegistrar](../registrar-tlds/get-get-registrar-tlds-by-registrar-api-v1-registrar-tlds-registrar-registrarid.md)
- [GET GetTickets](../support-tickets/get-get-tickets-api-v1-support-tickets.md)
- [GET GetActiveTlds](../tlds/get-get-active-tlds-api-v1-tlds-active.md)
- [GET GetAllTlds](../tlds/get-get-all-tlds-api-v1-tlds.md)
- [GET GetSecondLevelTlds](../tlds/get-get-second-level-tlds-api-v1-tlds-secondlevel.md)
- [GET GetTopLevelTlds](../tlds/get-get-top-level-tlds-api-v1-tlds-toplevel.md)
- [GET GetAllUsers](../users/get-get-all-users-api-v1-users.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

