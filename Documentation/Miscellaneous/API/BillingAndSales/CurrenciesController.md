# Currencies API Documentation

## Overview

Manages currency exchange rates and currency conversions

## Base URL

```
https://api.example.com/api/v1/Currencies
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles depending on the endpoint.

## Endpoints

### GET /api/v1/Currencies/rates

Retrieves all currency exchange rates in the system.

**Authorization:** Admin, Finance

**Response:**
- 200: Returns the list of currency exchange rates
- 401: If user is not authenticated
- 403: If user doesn't have required role (Admin, Finance)
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "baseCurrency": "EUR",
    "targetCurrency": "USD",
    "rate": 1.05,
    "effectiveDate": "2023-01-01T00:00:00Z",
    "expiryDate": null,
    "source": "Manual",
    "isActive": true
  }
]
```

### GET /api/v1/Currencies/rates/active

Retrieves all active currency exchange rates.

**Authorization:** None (authenticated users)

**Response:**
- 200: Returns the list of active currency exchange rates
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "baseCurrency": "EUR",
    "targetCurrency": "USD",
    "rate": 1.05,
    "effectiveDate": "2023-01-01T00:00:00Z",
    "expiryDate": null,
    "source": "Manual",
    "isActive": true
  }
]
```

### GET /api/v1/Currencies/rates/{id}

Retrieves a specific currency exchange rate by its unique identifier.

**Authorization:** Admin, Finance

**Parameters:**
- `id` (path): The unique identifier of the exchange rate

**Response:**
- 200: Returns the currency exchange rate data
- 401: If user is not authenticated
- 403: If user doesn't have required role (Admin, Finance)
- 404: If exchange rate is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "baseCurrency": "EUR",
  "targetCurrency": "USD",
  "rate": 1.05,
  "effectiveDate": "2023-01-01T00:00:00Z",
  "expiryDate": null,
  "source": "Manual",
  "isActive": true
}
```

### GET /api/v1/Currencies/rates/exchange

Retrieves the current exchange rate between two currencies.

**Authorization:** None (authenticated users)

**Parameters:**
- `from` (query): The source currency code (ISO 4217, e.g., EUR)
- `to` (query): The target currency code (ISO 4217, e.g., USD)
- `date` (query, optional): Optional date for the exchange rate (defaults to current date)

**Response:**
- 200: Returns the exchange rate data
- 401: If user is not authenticated
- 404: If exchange rate is not found for the currency pair
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "baseCurrency": "EUR",
  "targetCurrency": "USD",
  "rate": 1.05,
  "effectiveDate": "2023-01-01T00:00:00Z",
  "expiryDate": null,
  "source": "Manual",
  "isActive": true
}
```

### GET /api/v1/Currencies/rates/pair

Retrieves all exchange rates for a specific currency pair.

**Authorization:** Admin, Finance

**Parameters:**
- `from` (query): The source currency code (ISO 4217, e.g., EUR)
- `to` (query): The target currency code (ISO 4217, e.g., USD)

**Response:**
- 200: Returns the list of exchange rates
- 401: If user is not authenticated
- 403: If user doesn't have required role (Admin, Finance)
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "baseCurrency": "EUR",
    "targetCurrency": "USD",
    "rate": 1.05,
    "effectiveDate": "2023-01-01T00:00:00Z",
    "expiryDate": null,
    "source": "Manual",
    "isActive": true
  }
]
```

### POST /api/v1/Currencies/rates

Creates a new currency exchange rate.

**Authorization:** Admin, Finance

**Request Body:**
```json
{
  "baseCurrency": "EUR",
  "targetCurrency": "USD",
  "rate": 1.05,
  "effectiveDate": "2023-01-01T00:00:00Z",
  "expiryDate": null,
  "source": "Manual"
}
```

**Response:**
- 201: Returns the newly created currency exchange rate
- 400: If the exchange rate data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role (Admin, Finance)
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "baseCurrency": "EUR",
  "targetCurrency": "USD",
  "rate": 1.05,
  "effectiveDate": "2023-01-01T00:00:00Z",
  "expiryDate": null,
  "source": "Manual",
  "isActive": true
}
```

### PUT /api/v1/Currencies/rates/{id}

Updates an existing currency exchange rate.

**Authorization:** Admin, Finance

**Parameters:**
- `id` (path): The unique identifier of the exchange rate to update

**Request Body:**
```json
{
  "rate": 1.06,
  "effectiveDate": "2023-01-01T00:00:00Z",
  "expiryDate": null,
  "source": "Manual",
  "isActive": true
}
```

**Response:**
- 200: Returns the updated currency exchange rate
- 400: If the exchange rate data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role (Admin, Finance)
- 404: If exchange rate is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "baseCurrency": "EUR",
  "targetCurrency": "USD",
  "rate": 1.06,
  "effectiveDate": "2023-01-01T00:00:00Z",
  "expiryDate": null,
  "source": "Manual",
  "isActive": true
}
```

### DELETE /api/v1/Currencies/rates/{id}

Deletes a currency exchange rate.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the exchange rate to delete

**Response:**
- 204: If the exchange rate was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role (Admin)
- 404: If exchange rate is not found
- 500: If an internal server error occurs

### POST /api/v1/Currencies/convert

Converts an amount from one currency to another.

**Authorization:** None (authenticated users)

**Request Body:**
```json
{
  "amount": 100.0,
  "fromCurrency": "EUR",
  "toCurrency": "USD",
  "rateDate": null
}
```

**Response:**
- 200: Returns the conversion result
- 400: If the conversion data is invalid
- 401: If user is not authenticated
- 404: If exchange rate is not found for the currency pair
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "originalAmount": 100.0,
  "fromCurrency": "EUR",
  "toCurrency": "USD",
  "exchangeRate": 1.05,
  "convertedAmount": 105.0,
  "rateDate": "2023-01-01T00:00:00Z"
}
```

### POST /api/v1/Currencies/rates/deactivate-expired

Deactivates all expired currency exchange rates.

**Authorization:** Admin, Finance

**Response:**
- 200: Returns the count of deactivated rates
- 401: If user is not authenticated
- 403: If user doesn't have required role (Admin, Finance)
- 500: If an internal server error occurs

**Response Body:**
```json
5
```

[Back to Controllers Index](index.md)