# Register Domain Sales Flow

## Overview
This document describes the complete domain registration sales flow for administrators and sales representatives selling domains to customers. The flow guides users through customer selection, registrar selection, domain availability checking, and order creation.

---

## User Roles
- **Admin**: Full access to register domains for any customer
- **Sales**: Can register domains for customers they manage

---

## Flow Diagram

```
[Start] ? [1. Customer Selection] ? [2. Registrar Selection] ? [3. Domain Entry] 
    ? [4. TLD Support Check] ? [5. Availability Check]
    ? {Available?}
        ?? Yes ? [8. Configure Registration] ? [9. Add to Order] ? [End]
        ?? No ? [6. Show Suggested Alternatives]
            ? [7. User Selects Suggested Domain?]
                ?? Yes ? [8. Configure Registration] ? [9. Add to Order] ? [End]
                ?? No ? [Return to Step 3]
```

---

## Detailed Steps

### Step 1: Enter Customer Number
**Purpose**: Identify the customer for whom the domain is being registered.

**User Action**:
- Enter the customer number in the input field
- Example: `1001`

**System Action**:
- Store customer ID for later use in order creation

**Validation**:
- Customer number must be a valid integer
- Customer must exist in the system

**API**: N/A (validated during order creation)

---

### Step 2: Select Registrar
**Purpose**: Choose the registrar that will handle the domain registration.

**User Action**:
- Click "Load" button to fetch available registrars
- Select registrar from dropdown

**System Action**:
- Fetch registrars from API
- Auto-select the default registrar (marked with `isDefault: true`)
- Display registrar name with "(Default)" indicator for the default registrar

**API Endpoint**: `GET /api/v1/Registrars`

**Response Example**:
```json
[
  {
    "id": 1,
    "name": "GoDaddy",
    "isDefault": true,
    "apiEndpoint": "https://api.godaddy.com",
    "isActive": true
  },
  {
    "id": 2,
    "name": "Namecheap",
    "isDefault": false,
    "apiEndpoint": "https://api.namecheap.com",
    "isActive": true
  }
]
```

**Default Behavior**:
- The registrar with `isDefault: true` is automatically focused/selected
- If no default is set, the first registrar in the list is selected

---

### Step 3: Enter Domain Name
**Purpose**: Specify the desired domain name to register.

**User Action**:
- Enter full domain name including TLD
- Examples: `example.com`, `mycompany.net`, `shop.io`

**Format**:
- Must include both domain name and TLD
- Format: `{name}.{tld}`

---

### Step 4: Check if TLD is Supported
**Purpose**: Verify that the selected registrar supports the TLD and retrieve pricing information.

**User Action**:
- Click "Check TLD & Pricing" button

**System Action**:
- Extract TLD from domain name (e.g., `com` from `example.com`)
- Query pricing API for the TLD and selected registrar
- Display pricing information if supported
- Show warning if TLD is not supported

**API Endpoint**: `GET /api/v1/RegisteredDomains/pricing/{tld}?registrarId={registrarId}`

**Request Parameters**:
- `tld` (path): Top-level domain (e.g., "com", "net", "io")
- `registrarId` (query, optional): ID of selected registrar

**Response Example** (Supported):
```json
{
  "tld": "com",
  "registrarId": 1,
  "registrarName": "GoDaddy",
  "registrationPrice": 12.99,
  "renewalPrice": 14.99,
  "transferPrice": 9.99,
  "currency": "USD",
  "minimumYears": 1,
  "maximumYears": 10
}
```

**Response** (Not Supported):
- HTTP 404 Not Found
- Display: "TLD .{tld} is not supported by the selected registrar."

**Display**:
- Success: "TLD .com supported. Price: $12.99 USD"
- Not Supported: Warning message

---

### Step 5: Check Domain Availability
**Purpose**: Determine if the domain is available for registration.

**User Action**:
- Click "Check Availability" button

**System Action**:
- Call availability check API
- Process response to determine availability status
- If available, proceed to Step 8
- If not available, proceed to Step 6

**API Endpoint**: `POST /api/v1/RegisteredDomains/check-availability`

**Request Body**:
```json
{
  "domainName": "example.com"
}
```

**Response Example** (Available):
```json
{
  "domainName": "example.com",
  "isAvailable": true,
  "message": "Domain is available for registration",
  "suggestedAlternatives": []
}
```

**Response Example** (Not Available):
```json
{
  "domainName": "example.com",
  "isAvailable": false,
  "message": "Domain is already registered",
  "suggestedAlternatives": [
    "example.net",
    "example.org",
    "exampleshop.com",
    "myexample.com",
    "example.io"
  ]
}
```

**Display**:
- Available: Green success alert with "Proceed" button
- Not Available: Red danger alert with message

---

### Step 6: Show Suggested Domain Alternatives
**Purpose**: Provide alternative domain options when the requested domain is not available.

**System Action**:
- Extract `suggestedAlternatives` array from availability check response
- Display list of suggested domains

**Display**:
- Section header: "Suggested alternatives:"
- Interactive list of suggested domains
- Each item is clickable

**User Interaction**:
- User can click on any suggested domain
- Clicking populates the domain name field with the selected suggestion
- Automatically proceeds to Step 7

---

### Step 7: User Selects Suggested Domain
**Purpose**: Allow user to choose an alternative domain.

**User Action**:
- Click on a suggested domain from the list
- OR manually enter a different domain and return to Step 3

**System Action** (if suggested domain selected):
- Populate domain name field with selected domain
- Automatically check TLD support (Step 4)
- Automatically check availability (Step 5)
- If available, proceed to Step 8
- If not available, show new suggestions (Step 6)

**Alternative Path**:
- User can manually modify domain name and return to Step 3
- User can select a different registrar and return to Step 2

---

### Step 8: Configure Registration Properties
**Purpose**: Set registration parameters and options.

**User Action**:
- Select registration period (years)
- Enable/disable Auto Renew
- Enable/disable Privacy Protection
- Add optional notes

**Configuration Options**:

#### Registration Period
- **Field**: Dropdown selection
- **Options**: 1, 2, 3, 5, or 10 years
- **Default**: 1 year

#### Auto Renew
- **Field**: Checkbox
- **Default**: Checked (enabled)
- **Description**: Automatically renew domain before expiration

#### Privacy Protection
- **Field**: Checkbox
- **Default**: Unchecked (disabled)
- **Cost**: +$10.00 per year
- **Description**: Hide registrant information from WHOIS database

#### Notes
- **Field**: Text area
- **Optional**: Yes
- **Purpose**: Add internal notes or customer-specific information

**Price Calculation**:
```
Base Price = TLD Registration Price × Years
Privacy Cost = $10.00 × Years (if enabled)
Total Amount = Base Price + Privacy Cost
```

**Example**:
- TLD Price: $12.99/year
- Years: 3
- Privacy: Enabled
- **Total**: (12.99 × 3) + (10.00 × 3) = $38.97 + $30.00 = **$68.97 USD**

**Display**:
- Real-time price calculation updates when user changes years or privacy protection
- Estimated Total displayed prominently

---

### Step 9: Add to Order
**Purpose**: Create the domain registration order for the customer.

**User Action**:
- Review configuration and pricing
- Click "Add to Order" button

**System Action**:
- Validate all required fields
- Submit order to API
- Display success message with order details
- OR display error message if order fails

**API Endpoint**: `POST /api/v1/RegisteredDomains/register-for-customer`

**Request Body**:
```json
{
  "customerId": 1001,
  "domainName": "example.com",
  "registrarId": 1,
  "years": 3,
  "autoRenew": true,
  "privacyProtection": true,
  "notes": "Customer requested privacy protection for business domain"
}
```

**Response Example** (Success):
```json
{
  "success": true,
  "orderNumber": "ORD-2024-001234",
  "orderId": 1234,
  "domainName": "example.com",
  "totalAmount": 68.97,
  "currency": "USD",
  "message": "Domain registration order created successfully",
  "expirationDate": "2027-12-15T00:00:00Z"
}
```

**Response Example** (Failure):
```json
{
  "success": false,
  "message": "Customer not found",
  "errors": [
    "Customer ID 1001 does not exist"
  ]
}
```

**Display**:
- Success: Green alert with order number and total amount
- Failure: Red alert with error message

**Validation**:
- Customer number must be provided
- Registrar must be selected
- Domain must be specified
- Pricing information must be available
- All required fields in request must be valid

---

## API Reference Summary

### 1. Get Registrars
**Endpoint**: `GET /api/v1/Registrars`  
**Purpose**: Fetch list of available registrars  
**Auth**: Required

### 2. Get TLD Pricing
**Endpoint**: `GET /api/v1/RegisteredDomains/pricing/{tld}?registrarId={id}`  
**Purpose**: Check if TLD is supported and get pricing  
**Auth**: Required

### 3. Check Domain Availability
**Endpoint**: `POST /api/v1/RegisteredDomains/check-availability`  
**Purpose**: Verify domain availability and get suggestions  
**Auth**: Required

### 4. Register Domain for Customer
**Endpoint**: `POST /api/v1/RegisteredDomains/register-for-customer`  
**Purpose**: Create domain registration order  
**Auth**: Required (Admin/Sales roles)

---

## Error Handling

### Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| Failed to load registrars | API error or authentication issue | Check credentials and retry |
| TLD not supported | Registrar doesn't support the TLD | Choose different registrar or TLD |
| Domain not available | Domain already registered | Select suggested alternative |
| Customer not found | Invalid customer number | Verify customer number |
| Pricing not available | TLD pricing not configured | Contact administrator |
| Order creation failed | Validation or system error | Check all fields and retry |

---

## Business Rules

1. **Customer Validation**: Customer must exist in the system before order creation
2. **Registrar Selection**: Default registrar is automatically selected
3. **TLD Support**: Domain TLD must be supported by selected registrar
4. **Domain Availability**: Domain must be available for registration
5. **Registration Period**: 1-10 years allowed
6. **Auto Renew**: Recommended to be enabled by default
7. **Privacy Protection**: Optional add-on at fixed $10/year
8. **Pricing**: Based on TLD and registrar combination

---

## User Experience Flow

### Successful Registration
1. Enter customer number ? 2. Select registrar ? 3. Enter domain ? 4. Check TLD (supported) ? 5. Check availability (available) ? 6. Configure (3 years, privacy) ? 7. Add to order ? **Success: Order ORD-2024-001234 created**

### Domain Not Available Flow
1. Enter customer number ? 2. Select registrar ? 3. Enter domain ? 4. Check TLD (supported) ? 5. Check availability (not available) ? 6. View suggestions ? 7. Select alternative ? 5. Check availability (available) ? 6. Configure ? 7. Add to order ? **Success**

### TLD Not Supported Flow
1. Enter customer number ? 2. Select registrar ? 3. Enter unusual.xyz ? 4. Check TLD (not supported) ? **Warning: Change registrar or TLD**

---

## Test Page Location

**Path**: `/tests/register-domain-test.html`

**Purpose**: Interactive test page for domain registration sales flow

**Features**:
- Step-by-step guided flow
- Real-time pricing calculations
- Alternative domain suggestions
- Visual feedback for each step
- Success/error messaging

---

## Related Documentation
- [Domain Registration Overview](./Domain-Registration-Overview.md)
- [Registrar Management](./Registrar-Management.md)
- [API Documentation](../../API/RegisteredDomains-API.md)
