# DonationHub

DonationHub is a donation management and crowdfunding REST API built with **ASP.NET Core**. The solution follows **Clean Architecture** and uses **CQRS with MediatR**, the **Result pattern**, centralized error handling, and automated unit testing.

## Architecture

The solution is organized into four projects under `src/`, with automated tests under `tests/`.

```text
DonationHub/
├── src/
│   ├── DonationHub.Api/
│   │   ├── Controllers/
│   │   ├── Exceptions/
│   │   ├── Shared/
│   │   └── ViewModels/
│   ├── DonationHub.Application/
│   │   ├── Abstractions/
│   │   ├── Behaviors/
│   │   ├── Features/
│   │   ├── Options/
│   │   └── Shared/
│   ├── DonationHub.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   ├── Interfaces/
│   │   ├── Primitives/
│   │   └── ValueObjects/
│   └── DonationHub.Infrastructure/
│       ├── Authentication/
│       ├── Identity/
│       ├── Interceptors/
│       ├── Migrations/
│       ├── Options/
│       ├── Persistence/
│       ├── Repositories/
│       └── Services/
├── tests/
│   ├── DonationHub.Application.Test/
│   └── DonationHub.Domain.Test/
├── DonationHub.slnx
└── README.md
```

### Project responsibilities

| Project | Responsibility |
|---|---|
| **Domain** | Business entities, value objects, enums, domain interfaces, and business rules. It has no dependency on the other application layers. |
| **Application** | Use cases organized as vertical slices, CQRS commands and queries, handlers, validators, abstractions, pipeline behaviors, and the Result pattern. |
| **Infrastructure** | Implements persistence, EF Core configurations and migrations, repositories, Unit of Work, Identity, authentication services, and database interceptors. |
| **Api** | HTTP entry point: controllers, request/response ViewModels, result-to-HTTP mapping, exception handling, and application configuration. |
| **Tests** | Unit tests for application behaviors and use cases, domain entities, and shared components. |

The dependency direction points inward: the Domain is independent, and outer layers implement or use abstractions defined by inner layers.

## Main design decisions

### CQRS and vertical slices

A **Command** represents an operation that changes application state; a **Query** retrieves data without changing state. Each use case is kept in its own feature slice, grouping its request, handler, validator, and related DTOs.

MediatR dispatches each request to its corresponding handler. This keeps controllers thin: they bind HTTP input, send a command or query, and translate the result into an HTTP response.

### Read/write separation

- **Writes** use the Repository and Unit of Work abstractions. This centralizes persistence operations and gives write use cases a consistent place to coordinate changes and saving.
- **Reads** can query through the DbContext (or an Application abstraction over it), using `AsNoTracking()` and projection with `Select()` to retrieve only the required data.

This avoids forcing read-only projections through a repository abstraction designed primarily around entity persistence. The trade-off is that query code is more closely shaped around the persistence model, so the Application layer should depend on an appropriate abstraction rather than directly referencing Infrastructure.

### Result pattern and exceptions

Expected application outcomes—such as validation failures, missing records, or concurrency conflicts—are represented as Result values rather than exceptions. The API maps these errors to the appropriate HTTP status and ProblemDetails response.

Exceptions remain appropriate for unexpected or exceptional situations, such as infrastructure failures or unhandled programming errors. The global exception handler logs the technical details and returns a sanitized response; internal exception details should not be exposed to API clients.

### MediatR pipeline behaviors and validation

Pipeline behaviors wrap request-handler execution. They can run cross-cutting logic before and/or after a handler, without duplicating that logic in every use case.

`ValidationBehavior` obtains the registered FluentValidation validators for the request and executes them before the handler. It collects validation failures and returns a failed response when any exist. In that case, it does **not** call `next`, so the handler is not executed. If validation succeeds, it calls `next` and allows the request to continue.

When multiple behaviors are registered, they form a nested pipeline. The registration order determines which behavior is outermost and which runs closer to the handler; the corresponding unwind/after-handler work runs in reverse order. The exact order should be checked in the application's MediatR registration.

Input-shape rules (required values, lengths, and formats) belong in validators. Business invariants belong in the Domain.

### Optimistic concurrency with RowVersion

Campaign updates use a `RowVersion` value so the API can detect when a client is editing an outdated version of a campaign.

The client sends the last version it received. The update flow compares it with the currently loaded campaign version and returns a concurrency-conflict error if they differ. EF Core should also configure the property with `IsRowVersion()` so the database checks the version at save time. If another update wins between loading and saving, EF Core reports a `DbUpdateConcurrencyException`, which should be translated into the campaign concurrency-conflict result (HTTP `409 Conflict`).

This is **optimistic** concurrency: simultaneous edits are allowed to proceed, and a conflict is detected when saving. It is suitable when collisions are relatively uncommon. **Pessimistic locking** is an alternative when contention is high or a workflow must reserve data while it is being processed; it can reduce conflicting writes but introduces lock contention, waiting, and possible deadlocks.

### CancellationToken

The request cancellation token should be forwarded from the controller through MediatR, handlers, repositories, and EF Core asynchronous operations.

If it is forwarded, downstream operations that observe the token can stop work after the client disconnects or the request is cancelled. If it is not forwarded, the running database operation may continue even though the caller is no longer waiting for the response, wasting resources.

### TimeProvider

Handlers obtain the current time through the injected `TimeProvider` instead of directly calling `DateTime.UtcNow`. Tests can supply a controlled time provider, making date-dependent behavior—such as campaign end-date validation—repeatable and deterministic.

## Technology and package decisions

- **.NET 10** (`net10.0`).
- **MediatR 12.5.0** is pinned as `[12.5.0]` to prevent an automatic upgrade to version 13 or later, which has different commercial licensing terms.
- **FluentAssertions 7.0.0** is pinned because it uses the Apache 2.0 license; version 8 and later use a commercial license.
- **Moq 4.18.4** is pinned as specified by the project task guide.
- **FluentValidation** provides request validation.
- **EF Core with SQL Server** provides persistence.
- **xUnit** is used for unit testing.

Keep the pinned package versions unless a project maintainer or mentor approves a change.

## Build, test, and run

Run build and test commands from the **solution root directory** (the directory containing `DonationHub.slnx`).

### Build the solution

```bash
dotnet build
```

Builds the projects included in the solution and reports compilation errors.

### Run the tests

```bash
dotnet test
```

Builds and runs the test projects included in the solution, then reports the test results.

### Run the API

First, change the working directory to the API project:

```bash
cd src/DonationHub.Api
```

Then start the application:

```bash
dotnet run
```

The API uses the URLs configured by its active launch profile or application configuration. In Development, use the configured Scalar/OpenAPI endpoint to explore and test the endpoints.

## Unit testing

Tests follow the **Arrange – Act – Assert (AAA)** pattern. The test projects cover:

- Domain entity rules and state transitions.
- Application command validators.
- Command-handler outcomes and persistence interactions.
- MediatR pipeline behavior, including validation short-circuiting.

Dependencies such as repositories and time providers can be substituted in unit tests so the behavior can be tested independently. Tests should focus on application and domain behavior rather than testing EF Core itself or simple property getters.

Run the complete suite with:

```bash
dotnet test
```

## Error responses

Expected failures are mapped from the application's error type to an HTTP status code. Error responses use **RFC 9457 ProblemDetails** and include an application error `code` and request `traceId`. Validation responses can also include field-level errors.

Unexpected exceptions are handled centrally. Technical details are logged, while clients receive a generic server-error response without internal implementation details.

## Review notes: alternatives and trade-offs

| Decision | Alternative | Reason for the chosen approach / trade-off |
|---|---|---|
| Commands and Queries (CQRS) | One service method for reads and writes | Separates state changes from reads and gives each use case a focused handler; it adds more request/handler types. |
| Repository/UoW for writes; direct no-tracking query path for reads | Route every operation through repositories | Keeps write persistence coordinated while allowing efficient read projections; query code needs a suitable abstraction and can be more persistence-aware. |
| Result for expected failures | Throw exceptions for business outcomes | Makes expected failures explicit and avoids using exceptions for normal control flow; callers must handle and map results. |
| Exceptions for unexpected failures | Return Result for every possible failure | Preserves a clear distinction between expected outcomes and unexpected faults; unexpected exceptions still need centralized handling and logging. |
| Pipeline validation | Validate manually in each handler/controller | Centralizes cross-cutting validation and can stop invalid requests before handlers run; behavior order depends on registration. |
| Optimistic concurrency | Pessimistic locking | Avoids holding locks during ordinary edits and detects conflicts at update time; a conflicting client must handle/retry the update. |
| CancellationToken propagation | Start operations without forwarding cancellation | Allows downstream work to stop when the request is cancelled; every asynchronous boundary must forward the token. |
| TimeProvider | Direct calls to `DateTime.UtcNow` | Makes time controllable in tests; requires injecting and using the clock abstraction. |

For the implementation details, the code and dependency-injection registrations are the source of truth—especially for pipeline registration order and EF Core concurrency configuration.
