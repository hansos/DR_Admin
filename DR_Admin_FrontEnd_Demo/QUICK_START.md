# Quick Start - Direct API Configuration

## TL;DR

The frontend now calls the backend API directly at `https://localhost:[YOUR_PORT]/api/v1` using JWT tokens stored in sessionStorage.

## What Changed

### Before

```
Frontend ? Proxy Controller ? Backend API
         (Session)         (JWT)
```

### After

```
Frontend ? Backend API
         (JWT in Authorization header)
```

## Key Files Modified

1. **`wwwroot/js/api-client.js`**
   
   - Base URL: `https://localhost:[YOUR_PORT]/api/v1`
   - Adds JWT token from sessionStorage to all requests
   - Stores token on login, clears on logout

2. **`wwwroot/js/auth-check.js`**
   
   - Calls API to revoke refresh token on logout
   - Clears sessionStorage

## Quick Test

### 1. Start Backend

```bash
cd DR_Admin
dotnet run
```

### 2. Start Frontend

```bash
cd DR_Admin_FrontEnd_Demo
dotnet run
```

### 3. Login

- Go to `https://localhost:7247/login.html`
- Login with credentials

### 4. Verify

Open Browser DevTools:

- **Console**: No CORS errors
- **Network**: Requests go to `https://localhost:[YOUR_PORT]/api/v1/*`
- **Application ? Session Storage**:
  - `authToken` ?
  - `refreshToken` ?
  - `username` ?
  - `roles` ?

## API Endpoints

All requests now go to: `https://localhost:[YOUR_PORT]/api/v1`

| Resource  | Method | Endpoint          |
| --------- | ------ | ----------------- |
| Login     | POST   | `/auth/login`     |
| Logout    | POST   | `/auth/logout`    |
| Customers | GET    | `/customers`      |
| Customers | GET    | `/customers/{id}` |
| Customers | POST   | `/customers`      |
| Customers | PUT    | `/customers/{id}` |
| Customers | DELETE | `/customers/{id}` |
| Users     | GET    | `/users`          |
| Users     | GET    | `/users/{id}`     |
| Users     | POST   | `/users`          |
| Users     | PUT    | `/users/{id}`     |
| Users     | DELETE | `/users/{id}`     |

## Common Issues

### CORS Error

**Fix**: Backend already configured, just ensure backend is running

### 401 Unauthorized

**Fix**: Login again, token might be expired

### Token Not Sent

**Fix**: Check browser console for JS errors in api-client.js

## Need More Info?

- **Architecture Details**: See `DIRECT_API_CONFIGURATION.md`
- **Complete Changes**: See `MIGRATION_SUMMARY.md`
- **Feature Guide**: See `CUSTOMER_USER_MANAGEMENT.md`
