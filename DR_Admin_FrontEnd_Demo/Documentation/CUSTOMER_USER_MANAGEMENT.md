# DR Admin Frontend Demo - Customer and User Management

## Overview

This demo frontend now includes comprehensive **Customer Management** and **User Management** features with full CRUD operations, pagination, sorting, and filtering capabilities.

## Features

### Customer Management

#### Customer List Page (`/customers.html`)
- **Authorization Required**: Customer.Read policy (Admin or Sales role)
- **Features**:
  - **Pagination**: Navigate through large datasets (10 items per page by default)
  - **Sorting**: Click column headers to sort by ID, Name, or Email
  - **Filtering**:
    - Search by name or email (with debounce)
    - Filter by status (Active/Inactive)
    - Filter by type (Company/Individual)
  - **Actions**: Edit and Delete (soft delete) operations
  - Real-time UI updates with loading states

#### Create Customer Page (`/customer-create.html`)
- **Authorization Required**: Customer.Write policy (Admin or Sales role)
- **Fields**:
  - Name * (required)
  - Email * (required)
  - Phone
  - Type (Individual/Company)
  - Currency (USD, EUR, GBP)
  - Tax ID
  - VAT Number
  - Notes
- Form validation and error handling
- Redirects to customer list on success

#### Edit Customer Page (`/customer-edit.html?id={customerId}`)
- **Authorization Required**: Customer.Write policy (Admin or Sales role)
- **Editable Fields**:
  - Name
  - Email
  - Phone
  - Status (Active/Inactive)
  - Notes
- Pre-populated with existing customer data
- Redirects to customer list on success

### User Management

#### User List Page (`/users.html`)
- **Authorization Required**: User.Read policy (Admin or Support role)
- **Features**:
  - **Pagination**: Navigate through large datasets
  - **Sorting**: Click column headers to sort
  - **Filtering**:
    - Search by username or email
    - Filter by status (Active/Inactive)
    - Filter by role (Admin, Sales, Support, Customer)
  - **Actions**: Edit and Delete operations
  - Displays user roles as badges

#### Create User Page (`/user-create.html`)
- **Authorization Required**: User.Write policy (Admin only)
- **Fields**:
  - Username * (required)
  - Email * (required)
  - Password * (required, minimum 6 characters)
  - Role (Admin, Sales, Support, Customer)
  - Customer ID (optional, links user to customer)
  - Status (Active/Inactive)
- Form validation and password requirements
- Redirects to user list on success

#### Edit User Page (`/user-edit.html?id={userId}`)
- **Authorization Required**: User.Write policy (Admin only)
- **Editable Fields**:
  - Username
  - Email
  - Role
  - Customer ID
  - Status
- Pre-populated with existing user data
- Redirects to user list on success

## Authorization & Security

### Important Notes

All customer and user management operations require:
1. **Authentication**: Valid JWT token stored in session
2. **Authorization**: Appropriate role-based permissions

### Permission Matrix

| Operation | Customer Management | User Management |
|-----------|-------------------|-----------------|
| View List | Customer.Read (Admin, Sales) | User.Read (Admin, Support) |
| View Details | Customer.Read (Admin, Sales) | User.Read (Admin, Support) |
| Create | Customer.Write (Admin, Sales) | User.Write (Admin) |
| Update | Customer.Write (Admin, Sales) | User.Write (Admin) |
| Delete | Customer.Delete (Admin) | User.Delete (Admin) |

### Backend API Endpoints

All operations call the backend API directly at `https://localhost:[YOUR_PORT]`:

#### Customers
- `GET https://localhost:[YOUR_PORT]/api/v1/customers?pageNumber=1&pageSize=10` - Get paginated customers
- `GET https://localhost:[YOUR_PORT]/api/v1/customers/{id}` - Get customer by ID
- `POST https://localhost:[YOUR_PORT]/api/v1/customers` - Create customer
- `PUT https://localhost:[YOUR_PORT]/api/v1/customers/{id}` - Update customer
- `DELETE https://localhost:[YOUR_PORT]/api/v1/customers/{id}` - Delete customer (soft delete)

#### Users
- `GET https://localhost:[YOUR_PORT]/api/v1/users?pageNumber=1&pageSize=10` - Get paginated users
- `GET https://localhost:[YOUR_PORT]/api/v1/users/{id}` - Get user by ID
- `POST https://localhost:[YOUR_PORT]/api/v1/users` - Create user
- `PUT https://localhost:[YOUR_PORT]/api/v1/users/{id}` - Update user
- `DELETE https://localhost:[YOUR_PORT]/api/v1/users/{id}` - Delete user

#### Authentication
- `POST https://localhost:[YOUR_PORT]/api/v1/auth/login` - Login to get JWT token
- `POST https://localhost:[YOUR_PORT]/api/v1/auth/logout` - Logout and revoke refresh token

## Technical Implementation

### Direct API Communication

The frontend now calls the backend API directly at `https://localhost:[YOUR_PORT]/api/v1`:
- **No proxy controllers** - API requests go directly from JavaScript to backend
- **JWT Token Handling** - Token stored in `sessionStorage` and included in Authorization header
- **CORS Enabled** - Backend configured to accept requests from frontend origin (`https://localhost:7247`)

### Authentication Flow

1. User logs in via `POST /api/v1/auth/login`
2. Backend returns `LoginResponseDto` containing:
   - `accessToken` - JWT token for API authentication
   - `refreshToken` - Token for refreshing expired access tokens
   - `username` - Authenticated user's username
   - `roles` - User's assigned roles
   - `expiresAt` - Token expiration timestamp
3. Frontend stores tokens in `sessionStorage`:
   - `authToken` - Access token
   - `refreshToken` - Refresh token
   - `username` - User identifier
   - `roles` - JSON-encoded roles array
4. All subsequent API calls include `Authorization: Bearer {authToken}` header
5. On logout, frontend calls `POST /api/v1/auth/logout` to revoke refresh token and clears sessionStorage

### Frontend Proxy Controllers (Deprecated)

~~Previously, the frontend used proxy controllers (`ApiCustomersController.cs`, `ApiUsersController.cs`) to forward requests. These are no longer needed as the frontend calls the backend API directly.~~

### JavaScript Modules

#### Customer Management
- `customers.js` - List, pagination, sorting, filtering
- `customer-create.js` - Create form handling
- `customer-edit.js` - Edit form with data loading

#### User Management
- `users.js` - List, pagination, sorting, filtering
- `user-create.js` - Create form handling
- `user-edit.js` - Edit form with data loading

#### Shared Utilities
- `api-client.js` - API wrapper with endpoints for:
  - `CustomerAPI` - CRUD operations
  - `UserAPI` - CRUD operations
- `utils.js` - Helper functions (date formatting, HTML escaping, etc.)

### Features Implementation

#### Pagination
- Client-side pagination (10 items per page)
- Previous/Next buttons
- Page number buttons with ellipsis for large datasets
- Pagination info display ("Showing 1-10 of 50")

#### Sorting
- Click column headers to toggle sort direction
- Visual indicators (arrow icons)
- Supports ascending/descending order
- Works with string and numeric fields

#### Filtering
- **Search**: Debounced text search (300ms delay)
- **Status**: Dropdown filter (Active/Inactive)
- **Type/Role**: Dropdown filter based on entity
- **Clear Filters**: Reset all filters with one click
- Filters are combined with AND logic

#### Delete (Soft Delete)
- Confirmation modal before deletion
- Displays entity name for verification
- Explains soft delete behavior (reversible by admin)
- Updates list automatically after deletion

## Usage Examples

### Accessing Customer Management

```javascript
// Navigate to customers page
window.location.href = '/customers.html';

// Create new customer
window.location.href = '/customer-create.html';

// Edit existing customer
window.location.href = '/customer-edit.html?id=123';
```

### Accessing User Management

```javascript
// Navigate to users page
window.location.href = '/users.html';

// Create new user
window.location.href = '/user-create.html';

// Edit existing user
window.location.href = '/user-edit.html?id=456';
```

### Using API Client

```javascript
// Get paginated customers
const response = await window.CustomerAPI.getCustomers(1, 10);

// Create customer
const newCustomer = await window.CustomerAPI.createCustomer({
    name: "Acme Corp",
    email: "contact@acme.com",
    phone: "555-1234",
    isCompany: true,
    preferredCurrency: "USD"
});

// Update customer
const updated = await window.CustomerAPI.updateCustomer(123, {
    name: "Acme Corporation",
    email: "info@acme.com",
    phone: "555-1234",
    isActive: true
});

// Delete customer
const deleted = await window.CustomerAPI.deleteCustomer(123);
```

## Error Handling

All operations include comprehensive error handling:
- Network errors display user-friendly messages
- Authentication failures prompt re-login
- Validation errors show specific field issues
- Server errors display generic error messages
- Success messages with auto-dismiss (5 seconds)

### Session Management

The login process:
1. Calls backend authentication API at `POST https://localhost:[YOUR_PORT]/api/v1/auth/login`
2. Receives JWT token and user information in the response
3. Stores tokens and user data in browser's `sessionStorage`:
   - `authToken` - JWT access token
   - `refreshToken` - Refresh token for obtaining new access tokens
   - `username` - Authenticated user's username
   - `roles` - User's assigned roles (JSON array)
4. Automatically includes `Authorization: Bearer {token}` header in all API calls
5. Token persists in sessionStorage until browser tab/window is closed or user logs out
6. On logout, calls backend to revoke refresh token and clears all sessionStorage data

## Responsive Design

All pages are fully responsive using Bootstrap 5:
- Mobile-friendly tables with horizontal scroll
- Responsive forms with proper grid layout
- Touch-friendly buttons and controls
- Optimized for screens from 320px to 4K

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Future Enhancements

Potential improvements:
- Server-side pagination for large datasets
- Advanced filtering (date ranges, multiple selection)
- Bulk operations (delete multiple items)
- Export to CSV/Excel
- Import from CSV
- Audit log viewing
- Inline editing
- Advanced search with operators

## Troubleshooting

### 401 Unauthorized Errors
- Ensure you're logged in
- Check that JWT token is stored in session
- Verify your account has the required role/permissions

### 403 Forbidden Errors
- Your account lacks the required permission policy
- Contact administrator to grant appropriate role

### Empty Lists
- Check filters - clear all filters to see all items
- Verify backend API is running and accessible
- Check browser console for API errors

### Forms Not Submitting
- Check for validation errors (red borders/messages)
- Ensure all required fields (marked with *) are filled
- Check browser console for JavaScript errors

## Development Notes

- All API calls go through proxy controllers for security
- JWT tokens are never exposed to client-side JavaScript
- XSS protection through HTML escaping
- CSRF protection through session-based authentication
- Soft deletes preserve data integrity
