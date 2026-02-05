# Domain Registration Page

## Overview
The register-domain.html page provides a complete customer-facing interface for domain name registration, implementing the flow described in DOMAIN_REGISTRATION_IMPLEMENTATION.md.

## Files Created

### 1. register-domain.html
Main registration page with:
- **Step 1: Domain Search** - Check domain availability
- **Step 2: Configure Registration** - Set registration options
- **Step 3: Confirmation** - View registration results

### 2. domainRegistrationClient.js
JavaScript API client providing methods for:
- Domain registration (customer self-service)
- Domain registration for customers (admin/sales)
- Domain availability checking
- TLD pricing information
- Available TLDs listing

## Features

### Domain Availability Check
- Real-time domain availability verification
- Displays availability status with visual feedback
- Shows suggested alternatives for unavailable domains
- Automatic pricing fetch for available domains

### Registration Configuration
- Select registration period (1-10 years)
- Enable/disable auto-renewal
- Add privacy protection (+$10/year)
- Add custom notes
- Real-time price calculation

### Pricing Display
- Dynamic total calculation based on:
  - Registration period
  - Privacy protection option
  - Base TLD pricing
- Currency display

### Available TLDs Browser
- View all available top-level domains
- See pricing for each TLD
- Identify registrar for each TLD

### Registration Confirmation
- Order number display
- Invoice amount
- Approval status (if applicable)
- Options to:
  - Return to dashboard
  - Register another domain

## User Flow

### Customer Self-Service Flow

1. Login -> User authenticates with JWT token containing CustomerId claim
2. Search Domain -> Enter domain name and check availability
3. Configure -> Set registration period, auto-renewal, privacy options
4. Review Pricing -> See calculated total cost
5. Register -> Submit registration request
6. Confirmation -> View order details and payment instructions

### API Integration

The page integrates with the following API endpoints:

```
POST /api/v1/RegisteredDomains/register
POST /api/v1/RegisteredDomains/check-availability
GET  /api/v1/RegisteredDomains/pricing/{tld}
GET  /api/v1/RegisteredDomains/available-tlds
```

## Authentication & Authorization

### Required Authentication
- All API calls require JWT authentication
- Token must be stored in localStorage.authToken or localStorage.accessToken

### Authorization Policies
- **Domain.Register** - Allows CUSTOMER, SALES, ADMIN to register domains
- Automatically extracts CustomerId from JWT claims for customer users

## Usage Examples

### Customer Registration
1. Navigate to /register-domain.html
2. Enter domain name (e.g., example.com)
3. Click "Check Availability"
4. If available, click "Proceed to Registration"
5. Configure options and click "Register Domain"

### Accessing from Dashboard
The domain registration page is linked from the main dashboard (index.html) via the "Register Domain" card.

## Configuration

### API Base URL
Configure in domainRegistrationClient.js:
```javascript
const API_BASE_URL = 'https://localhost:7201';
```

### Privacy Protection Price
Configured in the HTML page (line ~468):
```javascript
total += 10 * years; // $10/year for privacy protection
```

## Backend Requirements

### Settings (appsettings.json)
```json
{
  "DomainRegistration": {
    "RequireApprovalForCustomers": false,
    "RequireApprovalForSales": false,
    "DefaultRegistrarId": 1,
    "AllowCustomerRegistration": true,
    "MaxRegistrationYears": 10,
    "MinRegistrationYears": 1,
    "DefaultRegistrationYears": 1,
    "EnableAvailabilityCheck": false,
    "EnablePricingCheck": true
  }
}
```

### Database Requirements
1. **Default Registrar** - Ensure registrar with ID specified in DefaultRegistrarId exists and is active
2. **TLD Pricing** - Populate RegistrarTld table with pricing for available TLDs
3. **Customer** - User must exist in Customers table with matching CustomerId in JWT

## Styling & Design

### Visual Design
- Gradient background (purple/blue theme)
- Card-based layout with shadows
- Responsive Bootstrap grid
- Step indicator for progress tracking
- Color-coded feedback (green=available, red=unavailable)

### Responsive Design
- Mobile-friendly layout
- Adaptive button placement
- Collapsible sections
- Touch-friendly controls

## Error Handling

### Client-Side Validation
- Domain name format validation
- Required field validation
- Real-time input feedback

### Server-Side Error Handling
- Invalid domain names
- Customer not found
- Registrar not available
- Pricing not found
- Payment failures

### User Feedback
- Alert messages for errors
- Loading states for async operations
- Success confirmation with details

## Testing Checklist

### Pre-Registration Tests
- [ ] Domain availability check works
- [ ] Invalid domain names show error
- [ ] TLD pricing displays correctly
- [ ] Available TLDs list loads

### Registration Flow Tests
- [ ] Customer can register domain
- [ ] Auto-renewal setting persists
- [ ] Privacy protection adds cost
- [ ] Notes field accepts input
- [ ] Registration creates order
- [ ] Invoice is generated

### Error Scenarios
- [ ] Invalid token shows login redirect
- [ ] Missing CustomerId claim shows error
- [ ] Disabled customer registration shows error
- [ ] Invalid registrar shows error
- [ ] Network errors display appropriately

## Troubleshooting

### "Customer domain registration is currently disabled"
**Solution**: Set AllowCustomerRegistration = true in appsettings.json

### "Customer ID not found in authentication token"
**Solution**: Ensure JWT contains CustomerId claim for customer users

### "Pricing not found for TLD"
**Solution**: Add RegistrarTld record with pricing for the requested TLD

### "Registrar with ID X not found or inactive"
**Solution**: Check DefaultRegistrarId in settings and ensure registrar exists

### Domain availability always shows "not available"
**Solution**: 
- Check if domain already exists in your RegisteredDomains table
- Review EnableAvailabilityCheck setting
- Ensure external registrar integration is configured (if needed)

## Future Enhancements

### Planned Features
- [ ] Domain transfer support
- [ ] Bulk domain registration
- [ ] Domain renewal interface
- [ ] DNS management integration
- [ ] Contact information management
- [ ] Real-time external registrar availability
- [ ] Shopping cart for multiple domains
- [ ] Payment gateway integration
- [ ] Auto-complete domain suggestions

## Security Considerations

### Authentication
- JWT token validation on every request
- Secure token storage in localStorage
- Automatic redirect to login if unauthenticated

### Authorization
- Customer can only register for themselves
- Sales/Admin endpoints require elevated permissions
- Customer ID extracted from secure JWT claims

### Data Validation
- Client-side validation for user experience
- Server-side validation for security
- Domain name format validation
- SQL injection protection via Entity Framework

## Dependencies

### Frontend
- Bootstrap 5.x (CSS framework)
- Bootstrap JavaScript Bundle
- authGuard.js (authentication check)
- domainRegistrationClient.js (API client)

### Backend
- DR_Admin API (running on https://localhost:7201)
- Entity Framework Core
- Domain Registration Workflow
- Authorization Policies

## Related Documentation
- DOMAIN_REGISTRATION_IMPLEMENTATION.md - Backend implementation details
- DomainLifecycleWorkflows.md - Workflow documentation
- API documentation at /swagger endpoint

## Support
For issues or questions about domain registration:
1. Check backend logs in DR_Admin API project
2. Verify database configuration
3. Review appsettings.json settings
4. Test API endpoints directly via Swagger/Postman
