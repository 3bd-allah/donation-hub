# DonationHub

A RESTful donation management and crowdfunding platform built with **ASP.NET Core 10**, following **Clean Architecture**, **Vertical Slice Architecture**, and **Domain-Driven Design (DDD)** principles.

DonationHub provides the foundation for managing users, authentication, categories, and campaigns, with secure JWT authentication and refresh-token rotation.

## Table of Contents

- [Overview](#overview)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Implemented Features](#implemented-features)
- [Design Decisions](#design-decisions)

## Overview

DonationHub is designed to provide a secure and maintainable backend for a donation and crowdfunding platform.

The solution is organized into four projects with a strict dependency direction. The Application layer follows Vertical Slice Architecture, organizing features around business use cases rather than technical layers.

The application uses Entity Framework Core with SQL Server for persistence and ASP.NET Core Identity for user management.

## Technologies

- **Framework:** .NET 10
- **Language:** C#
- **Architecture:** Clean Architecture, Vertical Slice Architecture, CQRS
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity, JWT Bearer Authentication
- **API Documentation:** OpenAPI and Scalar
- **Validation:** Data Annotations and strongly typed Options
- **Dependency Injection:** Built-in ASP.NET Core DI

## Prerequisites

Make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) with a running Database Engine instance
- [Git](https://git-scm.com/downloads)
- [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (only required when managing migrations manually)

Verify your .NET installation:

```bash
dotnet --version
```

## Project Structure

The solution uses four projects under the `src` directory:

```text
DonationHub/
│
├── src/
│   ├── DonationHub.Api/
│   │   └── Controllers, Program.cs, API configuration
│   │
│   ├── DonationHub.Application/
│   │   └── Features/
│   │       ├── Auth/
│   │       └── Categories/
│   │
│   ├── DonationHub.Domain/
│   │   └── Entities, Enums, ValueObjects, Interfaces
│   │
│   └── DonationHub.Infrastructure/
│       └── Persistence, Configurations, Repositories,
│           Interceptors, Identity, Seeding
│
├── DonationHub.slnx
└── README.md
```

### Layer Responsibilities

| Project | Responsibility |
|---|---|
| **Domain** | Business entities, value objects, domain rules, and abstractions |
| **Application** | Use cases organized into vertical slices, commands, queries, DTOs, and application interfaces |
| **Infrastructure** | EF Core persistence, SQL Server, Identity implementation, repositories, interceptors, and data seeding |
| **Api** | HTTP endpoints, middleware, dependency injection composition root, and API documentation |

The dependency direction follows Clean Architecture:

```text
           Api
            |
            v
      Application
            ^
            |
     Infrastructure
            |
            v
          Domain
```

Infrastructure implements abstractions defined by the inner layers. Domain remains independent of external frameworks and infrastructure concerns.

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd DonationHub
```

### 2. Configure User Secrets

Sensitive configuration values are stored using **ASP.NET Core User Secrets** rather than committed to the repository.

Initialize User Secrets for the API project:

```bash
dotnet user-secrets init --project src/DonationHub.Api
```

Set the seeded Admin password:

```bash
dotnet user-secrets set "SeedAdmin:Password" "<your-admin-password>" --project src/DonationHub.Api
```

Set the JWT signing secret:

```bash
dotnet user-secrets set "Jwt:Secret" "<your-secret-key-at-least-32-characters>" --project src/DonationHub.Api
```

**Important:**

- Use your own strong Admin password.
- The JWT secret must contain at least 32 characters.
- Never commit real passwords, JWT signing keys, or other sensitive configuration values.
- Keep User Secrets local to your development environment.

### 3. Configure the Database

The application uses SQL Server with the following default connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.; Initial Catalog=DonationHubDB; Integrated Security=True; Trust Server Certificate=True"
  }
}
```

This configuration connects to the default local SQL Server instance and creates/uses a database named `DonationHubDB`.

If your SQL Server instance uses a different name, update `Data Source` accordingly.

For example:

```text
Data Source=YOUR_SERVER_NAME;
```

#### Database Migrations

EF Core migrations are already included in the repository.

**You do not need to run a migration command manually for the normal first-time setup.** The application automatically applies pending migrations at startup using:

```csharp
await dbContext.Database.MigrateAsync();
```

If you need to update the database manually, run:

```bash
dotnet ef database update --project src/DonationHub.Infrastructure --startup-project src/DonationHub.Api
```

If the EF Core CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Run the Application

From the solution root:

```bash
dotnet run --project src/DonationHub.Api --launch-profile http
```

The API will be available at:

```text
http://localhost:5119
```

**Scalar API Documentation:**

```text
http://localhost:5119/scalar/v1
```

Scalar provides interactive API documentation and allows you to execute requests directly against the running application. It is configured with Bearer authentication for testing protected endpoints.

**Note:** Scalar and the OpenAPI document are available in the Development environment only.


## Implemented Features

### Authentication and Identity

- User registration with ASP.NET Core Identity.
- User login and JWT access-token generation.
- Refresh-token generation and rotation.
- Refresh tokens are stored as hashes rather than plaintext.
- Refresh-token revocation during logout.
- Current authenticated user profile endpoint.
- Role-based authorization.
- Initial Admin and role seeding.
- Configuration-driven JWT settings and Admin credentials.

### Categories Management

- Create categories.
- Retrieve all categories.
- Retrieve a category by ID.
- Update categories.
- Delete categories.
- Bilingual Arabic and English category names.
- Public access to category read operations.
- Admin-only access to category write operations.
- Protection against deleting categories associated with campaigns.

Deleting a category that still has associated campaigns returns `409 Conflict`.

### Domain and Persistence

- Rich domain models with encapsulated state and private setters.
- Value objects and domain invariants.
- Entity Framework Core Fluent API configurations.
- SQL Server persistence and committed migrations.
- Generic Repository and Unit of Work.
- Soft deletion using `ISoftDeletable` and `SoftDeleteInterceptor`.
- Global query filters for soft-deleted entities.
- Strongly typed Options with startup validation.

## Design Decisions

### 1. Clean Architecture

The solution is divided into four projects: Domain, Application, Infrastructure, and Api.

**Why?**

- Separates business rules from infrastructure and presentation concerns.
- Makes the core business logic independent of external technologies.
- Improves maintainability and testability.
- Enforces dependency inversion.

**Alternative:** A single-project or traditional layered architecture.

**Trade-off:** Clean Architecture introduces additional projects, abstractions, and initial setup, but provides clearer boundaries as the application grows.

### 2. Vertical Slice Architecture

The Application layer organizes functionality by feature and use case rather than grouping all controllers, services, and DTOs into separate technical folders.

**Why?**

- Keeps related functionality together.
- Improves cohesion.
- Makes features easier to understand and maintain.
- Reduces unnecessary coupling between unrelated features.

**Alternative:** Traditional layered organization, where all services, DTOs, and validators are grouped separately.

**Trade-off:** Some abstractions may be repeated between features, but each feature remains more independent.

### 3. CQRS-Style Read/Write Separation

The application separates query operations from command operations.

**Writes:**

Use the Generic Repository and Unit of Work abstractions.

**Reads:**

Query `AppDbContext` directly using `AsNoTracking()` and project results into DTOs.

**Why?**

- Write operations use centralized persistence abstractions.
- Read operations avoid unnecessary change tracking.
- DTO projection retrieves only the required data.
- Avoids forcing read-only queries through repository abstractions when direct EF Core queries are more suitable.

**Alternative:** Using repositories for both reads and writes.

**Trade-off:** Read operations depend more directly on EF Core, but gain flexibility and avoid unnecessary abstraction overhead.

### 4. Rich Domain Model and Value Objects

Entities encapsulate their state using private setters and expose behavior through domain methods.

Value objects represent concepts that have value-based equality, such as `Money`.

**Why?**

- Protects business invariants.
- Prevents arbitrary changes to entity state.
- Keeps business rules close to the domain objects they protect.

**Alternative:** Anemic domain models with public setters and business logic implemented entirely in services.

**Trade-off:** Rich domain models require more deliberate entity design and EF Core configuration, but help prevent invalid domain states.

### 5. Soft Deletion with EF Core Interceptors

Soft deletion is implemented using `ISoftDeletable`, `SoftDeleteInterceptor`, and global query filters.

**Why?**

- Preserves records instead of physically deleting them.
- Centralizes soft-delete behavior.
- Automatically excludes deleted records from normal queries.

**Alternative:** Implementing soft-delete logic manually in every application feature.

**Trade-off:** Interceptors and global filters introduce additional EF Core configuration and require explicit handling when deleted records must be queried.

### 6. Strongly Typed Options and Startup Validation

Configuration is represented through strongly typed options classes with Data Annotation validation and `ValidateOnStart()`.

**Why?**

- Provides structured and strongly typed configuration.
- Detects missing or invalid configuration during startup.
- Avoids scattering configuration key strings throughout the application.

**Alternative:** Reading values directly through `IConfiguration`.

**Trade-off:** Requires additional options classes and validation configuration, but makes configuration safer and easier to maintain.

### 7. Refresh-Token Hashing and Rotation

Refresh tokens are stored as hashes. When a refresh token is used, the previous token is revoked and replaced with a new token.

**Why?**

- Prevents stored refresh-token values from being directly reused if the database is exposed.
- Limits the lifetime of individual refresh tokens.
- Prevents replay of an already-rotated token.

**Alternative:** Storing refresh tokens as plaintext or using long-lived tokens without rotation.

**Trade-off:** Requires additional persistence and token-lifecycle logic, but improves authentication security.

