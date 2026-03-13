# Customer Details

View and manage the complete profile for a specific customer, including contact persons, domains, internal notes, and change history.

## How to Access

Click any customer name from the [Customer List](customer-list.md). The page route is `customers/details`.

> **Note:** This page is not shown in the main navigation menu — it is reached by selecting a customer from the list.

## Page Layout

### Toolbar

| Button | Description |
|--------|-------------|
| **Back to list** | Return to the [Customer List](customer-list.md). |
| **Edit Customer** | Open the edit dialog to modify customer information. |
| **Add Contact Person** | Open the dialog to create a new contact person for this customer. |

### Customer Information

A card displaying the customer's full profile, split into two columns:

| Field | Description |
|-------|-------------|
| ID | Internal system identifier. |
| Reference | External reference code. |
| Customer Number | Customer account number. |
| Name | Legal or personal name. |
| Customer Name | Display / trade name. |
| Email | Primary email address. |
| Billing Email | Email used for invoices and payment communications. |
| Phone | Phone number. |
| Country | Customer's country. |
| Tax ID | Tax identification number. |
| VAT Number | VAT registration number. |
| Preferred Currency | Default currency for billing. |
| Preferred Payment Method | Default [Payment Instrument](../billing/payment-instruments.md). |
| Credit Limit | Maximum allowed outstanding balance. |
| Balance | Current account balance. |
| Created | Account creation date. |
| Notes | Free-text notes about the customer. |

A **Status** badge is displayed in the card header.

### Contact Persons

A table listing all contact persons associated with this customer:

| Column | Description |
|--------|-------------|
| Name | Contact person's full name. |
| Email | Email address. |
| Phone | Phone number. |
| Position | Job title or position. |
| Primary | Whether this is the primary contact. |
| Active | Whether the contact is currently active. |
| Actions | Edit, Delete. |

#### Adding / Editing a Contact Person

The contact person dialog includes the following fields:

| Field | Description |
|-------|-------------|
| First Name * | Contact's first name (required). |
| Last Name * | Contact's last name (required). |
| Email * | Contact email (required). |
| Phone * | Contact phone (required). |
| Position | Job title or role. |
| Department | Organizational department. |
| Notes | Additional notes. |

And the following toggle switches:

| Toggle | Description |
|--------|-------------|
| Primary | Mark as the primary contact for the customer. |
| Active | Enable or disable the contact. |
| Default Owner | Use as the default domain owner (registrant) contact. |
| Default Billing | Use as the default billing contact for domains. |
| Default Tech | Use as the default technical contact for domains. |
| Default Admin | Use as the default administrative contact for domains. |
| Domain Global | Apply as a global default across all of the customer's domains. |

### Customer Domains

A table listing all domains belonging to this customer:

| Column | Description |
|--------|-------------|
| Domain | Fully qualified domain name. |
| Status | Domain registration status. |
| Expires | Expiry date. |
| Hosting | Associated hosting plan (if any). |
| Additional Services | Other services linked to the domain. |
| Action | View domain details. |

### Internal Notes

Add and view internal notes that are not visible to the customer:

- Enter a note in the text area and click **Add**.
- Existing notes are listed with **Created** date, **Note** content, and **User** who added it.

### Customer Changes

An audit log of all changes made to this customer:

| Column | Description |
|--------|-------------|
| Occurred | Date and time of the change. |
| Type | Type of change (e.g., field update, status change). |
| Field | The field that was modified. |
| Old | Previous value. |
| New | New value. |
| User | User who made the change. |

## Related Pages

- [Customer List](customer-list.md)
- [Contact Persons](contact-persons.md)
- [Domain Details](../domains/domain-details.md)
- [Payment Instruments](../billing/payment-instruments.md)
- [Invoices](../billing/invoices.md)

[Back to Reseller Manual index](../index.md)
