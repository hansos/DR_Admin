# Direct API Configuration Guide

## Overview

The DR Admin Frontend Demo has been configured to call the backend API directly instead of using proxy controllers. This document explains the configuration and how it works.

## Architecture Change

### Before (Proxy-based)
```
Frontend (JS) ? Frontend Controller (C#) ? Backend API
              (Session Token)            (JWT Token)
```

### After (Direct API Calls)
```
Frontend (JS) ? Backend API
             (JWT Token in Authorization header)
```

## Configuration

### Backend API Settings

**Base URL**: `https://localhost:[YOUR_PORT]/api/v1`

The backend API is configured in `DR_Admin/appsettings.json` with CORS settings that allow the frontend origin:

```json
{
  "CorsSettings": {
    "PolicyName": "AllowSpecificOrigins",
    "AllowedOrigins": [
      "https://localhost:7247",  // Frontend URL
      // ... other origins
    ],
    "AllowedMethods": [ "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" ],
    "AllowedHeaders": [ "*" ],
    "AllowCredentials": true,
    "MaxAge": 600
  }
}
```

### Frontend API Client

The `api-client.js` file has been updated to:

1. **Direct API Calls**: Points to `https://localhost:[YOUR_PORT]/api/v1`
2. **JWT Token Handling**: Retrieves token from `sessionStorage` and includes it in the `Authorization` header
3. **Automatic Token Injection**: All API requests automatically include the bearer token

**Key Changes**:

```javascript
// API Base URL - calls backend API directly
const BASE_URL = 'https://localhost:[YOUR_PORT]/api/v1';

// Get JWT token from session storage
function getAuthToken() {
    return sessionStorage.getItem('authToken');
}

// Add Authorization header if token exists
if (token) {
    headers['Authorization'] = `Bearer ${token}`;
}
```

## Authentication Flow

### 1. Login Process

**Endpoint**: `POST https://localhost:[YOUR_PORT]/api/v1/auth/login`

**Request**:
```json
{
  "username": "user@example.com",
  "password": "password123"
}
```

**Response**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_string",
  "username": "user@example.com",
  "roles": ["Admin", "Sales"],
  "expiresAt": "2024-01-15T12:00:00Z"
}
```

**Storage** (in `sessionStorage`):
- `authToken` ? Access token
- `refreshToken` ? Refresh token
- `username` ? User's username
- `roles` ? JSON array of roles

### 2. API Requests

All subsequent API requests automatically include:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

### 3. Logout Process

**Endpoint**: `POST https://localhost:[YOUR_PORT]/api/v1/auth/logout`

**Request**:
```json
{
  "refreshToken": "refresh_token_string"
}
```

**Actions**:
1. Revokes refresh token on backend
2. Clears all sessionStorage data:
   - `authToken`
   - `refreshToken`
   - `username`
   - `roles`
3. Redirects to login page

## API Endpoints

### Authentication
- `POST /api/v1/auth/login` - Login to get JWT token
- `POST /api/v1/auth/logout` - Logout and revoke refresh token
- `POST /api/v1/auth/refresh` - Refresh access token using refresh token

### Customers
- `GET /api/v1/customers` - List all customers (with pagination)
- `GET /api/v1/customers/{id}` - Get customer by ID
- `POST /api/v1/customers` - Create new customer
- `PUT /api/v1/customers/{id}` - Update customer
- `DELETE /api/v1/customers/{id}` - Delete customer (soft delete)

### Users
- `GET /api/v1/users` - List all users (with pagination)
- `GET /api/v1/users/{id}` - Get user by ID
- `POST /api/v1/users` - Create new user
- `PUT /api/v1/users/{id}` - Update user
- `DELETE /api/v1/users/{id}` - Delete user

### Domains
- `GET /api/v1/domains` - List domains
- `GET /api/v1/domains/search?domain={name}` - Search for domain

### Orders
- `GET /api/v1/orders` - List orders

## Security Considerations

### Token Storage

**SessionStorage** (Current Implementation):
- ? Cleared when browser tab/window closes
- ? Not sent automatically with requests (must be added manually)
- ? Not accessible across tabs
- ?? Vulnerable to XSS attacks (like any client-side storage)

**Best Practices**:
- Keep tokens short-lived (backend configured for 60 minutes)
- Use refresh tokens for obtaining new access tokens
- Always use HTTPS in production
- Implement proper XSS protection (Content Security Policy, input sanitization)

### CORS Configuration

The backend CORS configuration ensures:
- Only specified origins can make API requests
- Credentials (cookies, authorization headers) are allowed
- Preflight requests are cached for 10 minutes

### Authorization

All endpoints are protected by:
1. **Authentication**: Valid JWT token required
2. **Authorization**: Role-based access control (RBAC)
3. **Policies**: Resource-specific permissions (e.g., Customer.Read, User.Write)

## Error Handling

### 401 Unauthorized
- **Cause**: Missing or invalid token
- **Frontend Action**: Shows "Authentication required" message
- **User Action**: User should log in again

### 403 Forbidden
- **Cause**: User lacks required permission
- **Frontend Action**: Shows "Access denied" message
- **User Action**: Contact admin for permission

### Network Errors
- **Cause**: Backend unreachable or CORS issues
- **Frontend Action**: Shows "Network error" message
- **User Action**: Check backend is running and CORS is configured

## Development Setup

### 1. Start Backend API
```bash
cd DR_Admin
dotnet run
```
Backend runs on: `https://localhost:[YOUR_PORT]`

### 2. Start Frontend
```bash
cd DR_Admin_FrontEnd_Demo
dotnet run
```
Frontend runs on: `https://localhost:7247`

### 3. Verify CORS Configuration
- Check `DR_Admin/appsettings.json` includes frontend URL in `CorsSettings.AllowedOrigins`
- Check browser console for CORS errors (should be none)

## Testing

### Using Browser DevTools

1. **Check Token Storage**:
   ```javascript
   // In browser console
   sessionStorage.getItem('authToken')
   ```

2. **Inspect API Requests**:
   - Open Network tab
   - Filter by "Fetch/XHR"
   - Look for requests to `https://localhost:[YOUR_PORT]/api/v1/`
   - Check Request Headers for `Authorization: Bearer ...`

3. **Test Authorization**:
   - Try accessing customer list (requires Customer.Read)
   - Try creating user (requires User.Write - Admin only)

### Common Issues

**Issue**: CORS error in browser console
**Solution**: Verify backend CORS settings include frontend URL (`https://localhost:7247`)

**Issue**: 401 Unauthorized on all requests
**Solution**: 
- Check token exists: `sessionStorage.getItem('authToken')`
- Verify login was successful
- Check token expiration

**Issue**: Token not being sent
**Solution**: Check `api-client.js` includes token in Authorization header

## Production Considerations

1. **Environment Variables**: Use environment-specific configuration files
   - `appsettings.Development.json` - Development settings
   - `appsettings.Production.json` - Production settings

2. **HTTPS Only**: Always use HTTPS in production

3. **Token Security**:
   - Reduce token lifetime in production
   - Implement token refresh mechanism
   - Consider using httpOnly cookies for additional security

4. **CORS Restrictions**:
   - Lock down CORS to specific production domains
   - Remove wildcard (*) configurations

5. **API Rate Limiting**: Implement rate limiting on backend API

6. **Monitoring**: Add logging for authentication failures and API errors

## Migration from Proxy Controllers

The following controllers are now **deprecated** and can be removed:
- `DR_Admin_FrontEnd_Demo/Controllers/ApiCustomersController.cs`
- `DR_Admin_FrontEnd_Demo/Controllers/ApiUsersController.cs`
- `DR_Admin_FrontEnd_Demo/Controllers/ApiAccountController.cs` (if not used for other purposes)

The frontend no longer uses session-based token storage on the server side. All authentication is handled client-side via `sessionStorage` with JWT tokens sent directly to the backend API.

## Summary

? **Direct API Communication**: Frontend calls backend directly at `https://localhost:[YOUR_PORT]/api/v1`  
? **JWT Authentication**: Tokens stored in sessionStorage and sent in Authorization header  
? **CORS Enabled**: Backend configured to accept requests from frontend origin  
? **No Proxy Layer**: Eliminated intermediate proxy controllers  
? **Stateless Architecture**: Frontend doesn't maintain server-side session state  

This architecture provides:
- **Better Performance**: Fewer network hops
- **Simpler Debugging**: Direct API calls are easier to trace
- **Standard REST API**: Can be consumed by any client (web, mobile, desktop)
- **Scalability**: Stateless design allows horizontal scaling
