# CosmicCore backend

The backend targets .NET 10 and is organized as a modular monolith with separate projects for the HTTP API, application use cases, domain model, and infrastructure integrations.

## Projects

- `src/Api` — ASP.NET Core HTTP entry point and composition root.
- `src/Application` — application use cases and interfaces.
- `src/Domain` — core entities and business rules.
- `src/Infrastructure` — persistence and external service implementations.
- `tests/UnitTests` — unit tests for application and domain behavior.

The API is the boundary for external clients, including the future CosmicCore MCP server. Product rules belong in Application/Domain; infrastructure adapters should implement application-owned interfaces. The API exposes `/health` and a first knowledge-document CRUD endpoint set at `/api/knowledge-documents`.

Knowledge documents currently store a title, text content, an optional HTTP(S) source URL, and created/updated timestamps. Persistence uses EF Core with SQL Server.

### Start SQL Server with Docker Compose

From `backend/`, copy `.env.example` to `.env` and replace the sample password with a unique strong local password. `.env` is ignored by Git. Then start SQL Server:

```sh
cp .env.example .env
docker compose up -d
docker compose ps
```

The Compose service persists database files in the `cosmiccore-sqlserver-data` volume. To stop SQL Server while keeping the data, run `docker compose down`. To permanently delete its data too, run `docker compose down -v`.

**Apple Silicon note:** this machine is ARM64. Microsoft publishes SQL Server Linux container images for x86-64 only; running the image through Docker Desktop's `linux/amd64` emulation is unsupported by Microsoft and may be slow or fail. For supported local SQL Server containers, use an x86-64 host. The compose file requests emulation so you can try it if Docker Desktop supports it.

Configure the API connection string before running EF commands or the API:

```sh
export ConnectionStrings__CosmicCore='Server=localhost,1433;Database=CosmicCore;User Id=sa;Password=<your-password>;Encrypt=True;TrustServerCertificate=True'
```

Use the same password as `MSSQL_SA_PASSWORD` in `backend/.env`. EF Core Code First owns the schema: create a migration when the model changes, then apply migrations to the running database. Install the EF tool once if needed (`dotnet tool install --global dotnet-ef --version 10.0.0`). From `backend/`:

```sh
dotnet ef migrations add InitialKnowledgeDocuments --project src/Infrastructure/CosmicCore.Infrastructure.csproj --startup-project src/Api/CosmicCore.Api.csproj --output-dir Persistence/Migrations
dotnet ef database update --project src/Infrastructure/CosmicCore.Infrastructure.csproj --startup-project src/Api/CosmicCore.Api.csproj
```

## Run locally

After the database is healthy and the connection string is set, restore and run the API from `backend/`:

```sh
dotnet restore CosmicCore.slnx
dotnet run --project src/Api/CosmicCore.Api.csproj
```

The API reports its health at `http://localhost:<port>/health` using the URL printed at startup.
