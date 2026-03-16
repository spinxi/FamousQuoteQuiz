# Famous Quote Quiz Backend
This is our little application. I will write down little details about it.

## Projects

- `FamousQuoteQuiz.Api` - HTTP layer, controllers, Swagger, middleware
- `FamousQuoteQuiz.Application` - feature services, contracts, validators
- `FamousQuoteQuiz.Domain` - entities and enums
- `FamousQuoteQuiz.Infrastructure` - EF Core, SQL Server, migrations, seed data

## Implemented features

### User management
- create user
- update user
- disable user
- delete user (guarded if history exists)
- list users with pagination and filtering

### Quote management
- create quote
- update quote
- delete quote
- list quotes

### Quiz
- start session
- get next question
- binary or multiple choice mode
- submit answer and persist attempt history

### Achievements
- list question attempts
- filter by user
- filter by correctness
- sorting

## API endpoints

### Users
- `GET /api/users`
- `POST /api/users`
- `PUT /api/users/{id}`
- `PATCH /api/users/{id}/disable`
- `DELETE /api/users/{id}`

### Quotes
- `GET /api/quotes`
- `POST /api/quotes`
- `PUT /api/quotes/{id}`
- `DELETE /api/quotes/{id}`

### Quiz
- `POST /api/quiz/sessions`
- `GET /api/quiz/sessions/{sessionId}/next`
- `POST /api/quiz/answers/submit`

### Achievements
- `GET /api/achievements/attempts`

## Notes

- Binary mode is interpreted as: “Is this quote by `author`?”
- Controllers are thin and delegate to Application services
- EF Core is kept in Infrastructure
- Seed data is applied on startup after migrations

## Run it llocally

```bash
dotnet build
dotnet run --project src/FamousQuoteQuiz.Api
```

## Migration command for database

```bash
dotnet ef migrations add InitialCreate   --project src/FamousQuoteQuiz.Infrastructure   --startup-project src/FamousQuoteQuiz.Api   --output-dir Persistence/Migrations
```