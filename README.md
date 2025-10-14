# Payment Gateway - Take-Home Exercise

## Purpose
This document explains the key design decisions and testing approach for this take-home exercise.

## Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Runtime**: .NET 8.0

---

## What Was Built

A payment gateway API with two endpoints:
- POST /api/payments - Process a payment
- GET /api/payments/{id} - Retrieve payment details

The API integrates with a bank simulator and stores payment data in memory.

---

## Architecture Overview

The application follows a three-layer pattern:

**Controller Layer**
- Handles HTTP requests and responses
- Validates input
- Returns HTTP status codes

**Service Layer (PaymentProcessor)**
- Contains business logic
- Calls the bank simulator
- Maps between DTOs and entities
- Handles errors

**Repository Layer**
- Stores payment data in memory
- Retrieves payment by ID

**Supporting Services:**
- BankService - Calls the bank simulator
- PaymentResponseFactory - Creates payment entities
- GlobalExceptionHandler - Handles exceptions centrally

---

## Request Flow Examples

### Example 1: Successful Payment

**Request:**
```
POST http://localhost:5067/api/payments

{
  "cardNumber": "2222405343248111",
  "expiryMonth": 12,
  "expiryYear": 2026,
  "currency": "USD",
  "amount": 10000,
  "cvv": "123"
}
```

**Response:**
```
200 OK

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Authorized",
  "cardNumberLastFour": 8111,
  "expiryMonth": 12,
  "expiryYear": 2026,
  "currency": "USD",
  "amount": 10000
}
```

**Security Notes:**
- CVV sent to bank but never stored
- Full card number sent to bank but only last 4 digits stored
- Authorization code stored internally but not exposed in API response

---

### Example 2: Declined Payment

**Request:**
```
POST http://localhost:5067/api/payments

{
  "cardNumber": "4111111111111112",
  "expiryMonth": 6,
  "expiryYear": 2027,
  "currency": "GBP",
  "amount": 5000,
  "cvv": "456"
}
```

**Response:**
```
200 OK

{
  "id": "7c9d8e6f-5a4b-3c2d-1e0f-9a8b7c6d5e4f",
  "status": "Declined",
  "cardNumberLastFour": 1112,
  "expiryMonth": 6,
  "expiryYear": 2027,
  "currency": "GBP",
  "amount": 5000
}
```

**Note:** HTTP 200 is returned because the API call succeeded. The "status" field indicates payment outcome.

---

### Example 3: Payment Not Found

**Request:**
```
GET http://localhost:5067/api/payments/99999999-9999-9999-9999-999999999999
```

**Response:**
```
404 Not Found

{
  "message": "Payment not found",
  "details": "Payment with ID 99999999-9999-9999-9999-999999999999 was not found",
  "timestamp": "2025-10-13T19:30:00Z"
}
```

---

### Example 4: Validation Error

**Request:**
```
POST http://localhost:5067/api/payments

{
  "cardNumber": "123",
  "expiryMonth": 12,
  "expiryYear": 2020,
  "currency": "XXX",
  "amount": 0,
  "cvv": "1"
}
```

**Response:**
```
400 Bad Request

{
  "errors": {
    "CardNumber": ["Card number must be between 14-19 digits"],
    "ExpiryMonth": ["Card expiry date must be in the future"],
    "Currency": ["Currency must be USD, EUR, or GBP"],
    "Amount": ["Amount must be greater than 0"],
    "Cvv": ["CVV must be 3 or 4 digits"]
  }
}
```

---


## How to Build and Run

**Build the solution:**
```bash
dotnet build
```

**Run the API:**
```bash
dotnet run --project src/PaymentGateway.Api
```

The API will be available at:
- HTTPS: https://localhost:7092
- HTTP: http://localhost:5067 (redirects to HTTPS)
- Swagger UI: https://localhost:7092/swagger

**Start the bank simulator (required for integration tests):**
```bash
docker-compose up -d
```

**Stop the bank simulator:**
```bash
docker-compose down
```

---

## How to Run Tests

**Run all tests:**
```bash
dotnet test
```

**Run with detailed output:**
```bash
dotnet test --verbosity normal
```

---

## Assumptions Made

### Storage
In-memory storage is acceptable for this exercise. Requirements state "test double repository is fine."

### Currencies
Support limited to USD, EUR, GBP as specified in requirements.

### Bank Availability
Bank simulator runs on localhost:8080 as specified in provided configuration.

### Card Security
CVV and full card number never persisted. This follows PCI DSS requirements.

### Payment IDs
GUID/UUID format is acceptable. Non-sequential and hard to enumerate.

---
