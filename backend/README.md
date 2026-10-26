# CosmicCore backend

The backend targets .NET 10 and is organized as a modular monolith with separate projects for the HTTP API, application use cases, domain model, and infrastructure adapters.

## Projects

- `src/Api` — ASP.NET Core HTTP entry point and composition root.
- `src/Application` — application use cases and interfaces.
- `src/Domain` — core entities and business rules.
- `src/Infrastructure` — EF Core persistence and external service implementations.
- `tests/UnitTests` — unit tests for application and domain behavior.

The current KnowledgeDocument CRUD endpoint is a backend learning slice. It uses EF Core with SQLite and applies migrations automatically in Development. The planned notes architecture will move canonical note content to an Obsidian vault adapter; SQLite via EF Core will remain for structured app data and rebuildable indexes.

## Local database

By default, the API creates its SQLite database under the current user's local application-data directory in a `CosmicCore` folder. The database is outside the repository. To use another SQLite database, configure `ConnectionStrings__CosmicCore`, for example:

```sh
export ConnectionStrings__CosmicCore='Data Source=/absolute/path/to/cosmiccore.db'
```

SQLite foreign keys are enabled, WAL mode is initialized at startup, and the busy timeout is five seconds. SQLite allows only one writer at a time; keep writes short. Do not copy only the main database file while WAL mode is active for a backup. Use SQLite's backup facility and back up the Obsidian vault separately.

## Migrations

EF Core migrations own the database schema. Development startup applies migrations automatically. Install the EF tool once if needed:

```sh
dotnet tool install --global dotnet-ef --version 10.0.13
```

From `backend/`, add migrations with:

```sh
dotnet ef migrations add MigrationName --project src/Infrastructure/CosmicCore.Infrastructure.csproj --startup-project src/Api/CosmicCore.Api.csproj --output-dir Persistence/Migrations
```

The Development API applies migrations on startup. For future deployment, use a deliberate migration step rather than applying schema changes automatically from every production API instance.

## Run locally

From `backend/`:

```sh
dotnet restore CosmicCore.slnx
dotnet run --project src/Api/CosmicCore.Api.csproj
```

The API reports liveness at `http://localhost:<port>/health` using the URL printed at startup. The initial CRUD endpoints are under `/api/knowledge-documents`.
