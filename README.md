# FamousQuoteQuiz Backend

ASP.NET Core Web API backend for the Famous Quote Quiz test task.

This repository owns the server-side domain model, business rules, persistence, and administrative APIs used by the separate frontend repository.

## Scope

The backend covers:

- quiz session lifecycle
- question generation for `Binary` and `MultipleChoice` modes
- answer validation and attempt recording
- user management
- quote management
- achievements / attempt history review

This repository is intended to be delivered together with the separate frontend repository:

- Frontend repo: `https://github.com/spinxi/FamousQuoteQuiz.Frontend`

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server LocalDB
- FluentValidation

## Solution Layout

Main solution:

- [`FamousQuoteQuiz.sln`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/FamousQuoteQuiz.sln)

Projects:

- `src/FamousQuoteQuiz.Api`
  Web API host, controllers, middleware, startup wiring
- `src/FamousQuoteQuiz.Application`
  application services, contracts, validators, business logic
- `src/FamousQuoteQuiz.Domain`
  domain entities and enums
- `src/FamousQuoteQuiz.Infrastructure`
  EF Core persistence, migrations, seeding, dependency registration
- `tests/FamousQuoteQuiz.Application.Tests`
  focused business-rule tests for core application services

## Architecture

The backend uses a layered architecture with clear responsibility boundaries.

### Domain

Contains the core entities and enums only.

Examples:

- `User`
- `Quote`
- `GameSession`
- `QuestionAttempt`
- `QuizMode`

### Application

Contains the main use cases and business rules.

Examples:

- `QuizService`
- `UserService`
- `QuoteService`
- `QuestionAttemptService`

This layer is where quiz behavior and administrative rules are enforced.

### Infrastructure

Contains framework-specific implementation details:

- `AppDbContext`
- entity configurations
- migrations
- seed logic

### Api

Contains HTTP delivery concerns:

- controllers
- JSON setup
- middleware
- CORS
- Swagger

## Business Rules

Important rules enforced by the backend include:

- disabled users cannot start a quiz session
- starting a new session deactivates previous active sessions for that user
- a quote should not repeat within the same session
- inactive sessions cannot continue
- sessions end when no more active quotes are available
- users with historical game data cannot be deleted
- quotes referenced by attempts cannot be deleted

Primary implementations:

- [`QuizService.cs`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/src/FamousQuoteQuiz.Application/Features/Quiz/QuizService.cs)
- [`UserService.cs`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/src/FamousQuoteQuiz.Application/Features/Users/UserService.cs)
- [`QuoteService.cs`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/src/FamousQuoteQuiz.Application/Features/Quotes/QuoteService.cs)

## API Surface

### Quiz

Base route:

- `/api/quiz`

Endpoints:

- `POST /api/quiz/sessions`
- `GET /api/quiz/sessions/{sessionId}/next`
- `POST /api/quiz/answers/submit`

### Users

Base route:

- `/api/users`

Endpoints:

- `GET /api/users`
- `POST /api/users`
- `PUT /api/users/{id}`
- `PATCH /api/users/{id}/disable`
- `PATCH /api/users/{id}/enable`
- `DELETE /api/users/{id}`

### Quotes

Base route:

- `/api/quotes`

Endpoints:

- `GET /api/quotes`
- `POST /api/quotes`
- `PUT /api/quotes/{id}`
- `DELETE /api/quotes/{id}`

### Achievements

Base route:

- `/api/achievements`

Endpoints:

- `GET /api/achievements/attempts`

## Database

The backend is configured to use SQL Server LocalDB.

Configuration source:

- [`appsettings.json`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/src/FamousQuoteQuiz.Api/appsettings.json)

Default target:

- server: `(localdb)\MSSQLLocalDB`
- database: `FamousQuoteQuizDb`

On startup, the API:

- applies EF Core migrations automatically
- seeds initial data if the database is empty

Startup wiring:

- [`Program.cs`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/src/FamousQuoteQuiz.Api/Program.cs)

Seed logic:

- [`DataSeeder.cs`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/src/FamousQuoteQuiz.Infrastructure/Persistence/Seed/DataSeeder.cs)

## Seed Data

The project seeds sample records to make local review immediate:

- sample users
- sample quotes
- a sample finished session
- sample attempts

This makes the admin and achievements flows usable without manual preparation.

## Error Handling

The API uses centralized exception handling middleware.

Middleware:

- [`ExceptionHandlingMiddleware.cs`](https://github.com/spinxi/FamousQuoteQuiz/blob/master/src/FamousQuoteQuiz.Api/Middleware/ExceptionHandlingMiddleware.cs)

Typical behavior:

- `400` for validation errors
- `400` for business rule violations
- `404` for missing resources
- `500` for unexpected server failures

This keeps controller logic thin and returns consistent error shapes to the frontend.

## Running Locally

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB

### Commands

From the repository root:

```bash
dotnet restore
dotnet build
dotnet run --project src/FamousQuoteQuiz.Api
```

Swagger is enabled in development.

## Tests

This repository includes a focused application test project:

- `tests/FamousQuoteQuiz.Application.Tests`

The current test suite targets high-value business rules such as:

- deactivating previous sessions when a new session starts
- blocking disabled users from starting sessions
- correct vs incorrect answer evaluation
- ending sessions when no more quotes are available
- preventing deletion of users with game history

To run tests:

```bash
dotnet test
```

## Review / Demo Flow

A quick backend review path is:

1. start the API
2. open Swagger
3. verify user, quote, and achievements endpoints
4. start a quiz session
5. fetch the next question
6. submit an answer
7. confirm the attempt appears in achievements
8. disable and re-enable a user

## Design Choices

### Database-backed session state

Quiz progress is derived from `GameSessions` and `QuestionAttempts` instead of in-memory session storage.

Why:

- simpler to reason about
- survives restarts
- aligns better with horizontal scalability

### Simple randomization strategy

Random selection uses `Guid.NewGuid()` ordering.

Why:

- concise
- readable
- acceptable for the scale of this task

Tradeoff:

- not ideal for very large datasets

### Historical answer preservation

Multiple-choice options are stored as serialized JSON for each attempt.

Why:

- preserves exactly what the user saw
- avoids unnecessary relational complexity for a read-mostly concern

## Limitations

- authentication and authorization are not implemented
- no leaderboard or advanced analytics views
- randomization is pragmatic rather than highly optimized
- intended to work with a separate frontend repository rather than serve UI itself

## Submission Notes

For final delivery, this backend should be provided together with:

- the separate frontend repository
- the database backup or SQL export
- short instructions describing how to run both parts together