# DroneRental API

DroneRental is a portfolio backend project build with C# and ASP.NET Core Web API.

The application allows user to rent drones, while Admin users can manage drones and rental records. The project demonstrates REST API design, Entity Framework Core, JWT authentication, role-based authorization, ownership rules, filtering, pagination and integration testing.

## TechStack

- C#
- ASP.NET CORE Web API
- Entity Framework Core
- SQLite
- JWT Bearer Authentication
- Swagger/ OpenAPI
- xUnit
- Microsoft.AspNetCore.Mvc.Testing
- SQLite in-memory database for integration tests
- Git/ GitHub

## Solution Structure

DroneRental.Api
DroneRental.Core
DroneRental.Infrastructure
DroneRental.Api.Tests

## Main Features
- Drone CRUD
- Rental creation
- Rentalstatus lifecycle:
-- Acticve
-- Cancelled
-- Completed
- Business cancellation endpoint
- Admin-only technical rental deletion
- JWT authentication
- Admin and Customer roles
- Rental ownership rules
- Integration tests for key API flows
- CORS configuration for frontend development
- Develipment seed data for easier local testing

## Running the Backend

From the solution roor, run:

dotnet build
dontet run --project DroneRental.Api

After the API starts, open Swagger in the browser.

The exact local URL may depend on your launch profile, but it will usually look similar to:

https://localhost:{port}/swagger

## Database

The project uses SQLite for local development.

In the Development enviroment, the application applies pending EF Core migrations and seeds example data automatically.

The development seed data includes:

- example Admin account,
- example Customer account,
- example drones,
- example rentals with different statuses.

Seed data is intended only for local development and demo purposes.

## Example Accounts

### Admin

Email: admin@dronerental.dev
Password: Admin123!

### Customer

Email: customer@dronerental.dev
Password: Customer123!

These accounts are created only by the development seeder.

## Authentication

The API uses JWT Bearer Authentication.

After logging in, copy the returned token and use it in the Authorization header:

Authorization: Bearer <token>

In Swagger, click Authorize and paste the token.

## Roles and Permissions

### Admin

Admin users can:
- create, update and delete drones,
- view all rentals,
- filter and paginate rentals,
- complete actice rentals,
- cancel rentals,
- technically delete rental records.

### Customer

Customer users can:
- create rentals,
- view their own rentals,
- cancel their own rentals.

Customer users cannot:
- view all rentals,
- complete rentals,
- delete rentals using the technical DELETE endpoint,
- modify other users' rentals.

## Main API Endpoints

### Auth
POST /api/auth/register
POST /api/auth/login

### Drones
GET	   /api/drones/
GET    /api/drones/{id}
POST   /api/drones
PUT    /api/drones/{id}
DELETE /api/drones/{id}

Drone management endpoints are intended for Admin users.

### Rentals

GET    /api/rentals
GET    /api/rentals/{id}
GET    /api/rentals/my
POST   /api/rntals
POST   /api/rentals/{id}/cancel
POST   /api/rentals/{id}/complete
DELETE /api/rentals/{id}

## Rental Business Rules

- New rentals are created with `Active` status.
- A rental can be cancelled through:

POST /api/rentals/{id}/cancel

- A cancelled rental does not block future rentals for the same drone and time peroid.
- Only active rentals can be cancelled.
- Only Admin users can complete rentals.
- Cancelled rentals cannot be completed.
- Completed rentals cannot be cancelled.
- Technical deletion is available only for Admin users:

DELETE /api/rentals/{id}

The DELETE endpoint is not the normal business cancellation flow. It physically removes a rental record and is intended only as an administrative/technical operation.

## Rental Filtering and Pagination

Admin users can retrieve rentals with filtering and pagination:

GET /api/rentals

Supported query parameters:

status,
droneId,
customerEmail,
page,
pageSize

Example requests:

GET /api/rentals?page=1&pageSize=10
GET /api/rentals?status=Active
GET /api/rentals?customerEmail=customer@dronerental.dev
GET /api/rentals?status=Cancelled&page=1&pageSize=5

The paginated response includes:

items,
totalCount,
page,
pageSize,
totalPages

## CORS

The backend is configured to allow requests from a local Vite frontend:

http://localhost:5173

This prepares the API for a future React + TypeScript frontend.

## Running Tests

From the solution root, run:

dotnet test

The test project uses:
- xUnit,
- Microsoft.AspNetCore.Mvc.Testing,
- WebApplicationFactory,SQLite in-memory database,
- test JWT configuration.

Current integration test areas include:

- creating rentals,
- cancelling rentals,
- completing rentals,
- getting rentals with filtering and pagination,
- deleting rentals as an Admin-only technical operation.

## Current Project Status

Completed backend stages include:

- drone CRUD,
- rental creation,
- DTO's,
- validation
- rental statuses,
- cancellation,
- completion,
- JWT authentication,
- roles and ownership,
- integration testing,
- rental filtering and pagination,
- Admin-only rental deletion,
- CORS for local frontend development,
- development seed data.

The next planned stage is starting the frontend, likely with:

- React,
- TypeScript
- Vite.