# TLD Pricing API - Quick Reference

## ?? Base URLs

- **Cost Pricing:** `/api/v1/registrar-tld-cost-pricing`
- **Sales Pricing & Operations:** `/api/v1/tld-pricing`

---

## ?? Registrar Cost Pricing Endpoints

### Get Cost Pricing History
```http
GET /api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}?includeArchived=false
Authorization: Bearer {token}
```

### Get Current Cost Pricing
```http
GET /api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}/current?effectiveDate=2025-01-15
Authorization: Bearer {token}
```

### Get Future Cost Pricing
```http
GET /api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}/future
Authorization: Bearer {token}
```

### Create Cost Pricing
```http
POST /api/v1/registrar-tld-cost-pricing
Authorization: Bearer {token}
Content-Type: application/json

{
  "registrarTldId": 1,
  "effectiveFrom": "2025-03-01T00:00:00Z",
  "effectiveTo": null,
  "registrationCost": 8.50,
  "renewalCost": 9.00,
  "transferCost": 8.50,
  "privacyCost": 1.50,
  "firstYearRegistrationCost": 7.99,
  "currency": "USD",
  "isActive": true,
  "notes": "Q2 2025 pricing"
}
```

### Update Future Cost Pricing
```http
PUT /api/v1/registrar-tld-cost-pricing/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "effectiveFrom": "2025-03-15T00:00:00Z",
  "registrationCost": 8.75,
  "renewalCost": 9.00,
  "transferCost": 8.75,
  "currency": "USD",
  "isActive": true
}
```

### Delete Future Cost Pricing
```http
DELETE /api/v1/registrar-tld-cost-pricing/{id}
Authorization: Bearer {token}
```

---

## ?? TLD Sales Pricing Endpoints

### Get Sales Pricing History
```http
GET /api/v1/tld-pricing/sales/tld/{tldId}?includeArchived=false
Authorization: Bearer {token}
```

### Get Current Sales Pricing
```http
GET /api/v1/tld-pricing/sales/tld/{tldId}/current?effectiveDate=2025-01-15
Authorization: Bearer {token}
```

### Create Sales Pricing
```http
POST /api/v1/tld-pricing/sales
Authorization: Bearer {token}
Content-Type: application/json

{
  "tldId": 1,
  "effectiveFrom": "2025-03-01T00:00:00Z",
  "effectiveTo": null,
  "registrationPrice": 12.00,
  "renewalPrice": 12.00,
  "transferPrice": 12.00,
  "privacyPrice": 2.00,
  "firstYearRegistrationPrice": 9.99,
  "currency": "USD",
  "isPromotional": true,
  "promotionName": "Spring Sale 2025",
  "isActive": true,
  "notes": "Limited time offer"
}
```

### Update Future Sales Pricing
```http
PUT /api/v1/tld-pricing/sales/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "effectiveFrom": "2025-03-01T00:00:00Z",
  "registrationPrice": 14.00,
  "renewalPrice": 14.00,
  "transferPrice": 14.00,
  "currency": "USD",
  "isPromotional": false,
  "isActive": true
}
```

### Delete Future Sales Pricing
```http
DELETE /api/v1/tld-pricing/sales/{id}
Authorization: Bearer {token}
```

---

## ?? Pricing Calculation Endpoint

### Calculate Final Pricing
```http
POST /api/v1/tld-pricing/calculate
Content-Type: application/json

{
  "tldId": 1,
  "resellerCompanyId": 5,
  "operationType": "Registration",
  "years": 1,
  "isFirstYear": true,
  "targetCurrency": "USD"
}
```

**Response:**
```json
{
  "tldExtension": "com",
  "basePrice": 12.00,
  "discountAmount": 1.20,
  "finalPrice": 10.80,
  "currency": "USD",
  "isPromotionalPricing": false,
  "isDiscountApplied": true,
  "discountDescription": "10% discount",
  "selectedRegistrarId": 3,
  "selectedRegistrarName": "Namecheap"
}
```

**Operation Types:** `Registration`, `Renewal`, `Transfer`

---

## ?? Margin Analysis Endpoints

### Calculate Margin for TLD
```http
GET /api/v1/tld-pricing/margin/tld/{tldId}?operationType=Registration&registrarId=3
Authorization: Bearer {token}
```

**Response:**
```json
{
  "tldId": 1,
  "tldExtension": "com",
  "registrarId": 3,
  "registrarName": "Namecheap",
  "operationType": "Registration",
  "cost": 8.50,
  "price": 12.00,
  "marginAmount": 3.50,
  "marginPercentage": 41.18,
  "costCurrency": "USD",
  "priceCurrency": "USD",
  "isNegativeMargin": false,
  "isLowMargin": false,
  "alertMessage": null
}
```

### Get Negative Margin Report
```http
GET /api/v1/tld-pricing/margin/negative
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "tldId": 5,
    "tldExtension": "xyz",
    "cost": 12.00,
    "price": 9.99,
    "marginAmount": -2.01,
    "marginPercentage": -16.75,
    "isNegativeMargin": true,
    "alertMessage": "CRITICAL: Negative margin detected! Cost (USD 12.00) exceeds price (USD 9.99). Loss: 2.01"
  }
]
```

### Get Low Margin Report
```http
GET /api/v1/tld-pricing/margin/low
Authorization: Bearer {token}
```

Returns all TLDs with margins below threshold (includes negative margins).

---

## ??? Archive Management Endpoint

### Archive Old Pricing Data
```http
POST /api/v1/tld-pricing/archive
Authorization: Bearer {token}
```

**Response:**
```json
{
  "costPricingArchived": 45,
  "salesPricingArchived": 32,
  "discountsArchived": 12,
  "totalArchived": 89
}
```

Archives pricing older than retention policy:
- Cost Pricing: 7 years (default)
- Sales Pricing: 5 years (default)
- Discounts: 5 years (default)

---

## ?? Authorization Policies

| Policy | Permission | Endpoints |
|--------|-----------|-----------|
| `Pricing.Read` | View pricing data | All GET endpoints |
| `Pricing.Write` | Create/update pricing | POST, PUT endpoints |
| `Pricing.Delete` | Delete future pricing | DELETE endpoints |
| `Pricing.Admin` | Archive management | POST /archive |

**Note:** `/api/v1/tld-pricing/calculate` allows anonymous access for public price quotes.

---

## ?? Notes

### **Temporal Queries**
- **Current:** `effectiveDate` defaults to `DateTime.UtcNow`
- **Historical:** Provide past `effectiveDate` to query historical pricing
- **Future:** Use `/future` endpoints to see scheduled price changes

### **Editing Rules**
- Can only edit/delete **future** pricing (before `EffectiveFrom`)
- Configured via `AllowEditingFuturePrices` in appsettings
- Maximum schedule: `MaxScheduleDays` (default: 365 days)

### **Auto-Closing Pricing**
When creating pricing with `effectiveTo: null`, existing open pricing is automatically closed:
- Existing `EffectiveTo` set to new `EffectiveFrom - 1 second`
- Ensures no overlapping open pricing

### **Discount Stacking**
- **Promotional pricing** OR **customer discount** applied (not both)
- Configured via `AllowDiscountStacking` in appsettings (default: false)
- System chooses better deal for customer

### **Multi-Year Pricing**
- Price locked at purchase time
- Future price changes don't affect existing multi-year domains
- Only renewals use new pricing

---

## ??? Common Use Cases

### **Use Case 1: Schedule Black Friday Sale**
```http
POST /api/v1/tld-pricing/sales
{
  "tldId": 1,
  "effectiveFrom": "2025-11-28T00:00:00Z",
  "effectiveTo": "2025-12-01T23:59:59Z",
  "firstYearRegistrationPrice": 4.99,
  "registrationPrice": 12.00,
  "renewalPrice": 12.00,
  "currency": "USD",
  "isPromotional": true,
  "promotionName": "Black Friday 2025"
}
```

### **Use Case 2: Get Customer Quote**
```http
POST /api/v1/tld-pricing/calculate
{
  "tldId": 1,
  "resellerCompanyId": 5,
  "operationType": "Registration",
  "years": 3,
  "isFirstYear": true
}
```

### **Use Case 3: Check All Negative Margins**
```http
GET /api/v1/tld-pricing/margin/negative
```

### **Use Case 4: Update Registrar Costs**
```http
POST /api/v1/registrar-tld-cost-pricing
{
  "registrarTldId": 10,
  "effectiveFrom": "2025-02-01T00:00:00Z",
  "registrationCost": 9.25,
  "renewalCost": 9.75,
  "currency": "USD"
}
```

---

## ?? Related Documentation

- **TLD-Pricing-Implementation-Complete.md** - Full implementation details
- **TLD-Pricing-Migration-Guide.md** - Data migration guide
- **TLD-Pricing-Diagrams.md** - Architecture diagrams

---

**Version:** 1.0  
**Last Updated:** 2025-01-15
