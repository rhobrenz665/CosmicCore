---
name: dotnet-ef
description: Use for .NET backend, EF Core, SQLite persistence, or migration changes in CosmicCore.
---

# .NET and EF Core

Follow the architecture in the repository root AGENTS.md. Keep endpoints thin and put shared validation and use cases in Application. Infrastructure implements application-owned persistence interfaces.

- EF Core with SQLite owns structured application data. Obsidian Markdown remains canonical for note bodies; do not create a second editable copy in the database.
- Make schema changes through EF Core migrations. SQLite has provider-specific limitations, so inspect generated migrations and verify affected query behavior.
- Store timestamps consistently in UTC and check SQLite provider behavior for ordering/comparison of mapped types.
- Keep the DbContext scoped to a unit of work; do not run concurrent operations on one instance.
- Keep secrets and note/interview content out of logs and source control.
- For the current contract and feature scope, consult the matching Notes KB page only when needed; do not load unrelated pages.
